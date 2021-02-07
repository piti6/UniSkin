// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Globalization;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;


namespace UniSkin.UnityEditorInternalBridge
{
    internal class BaseInspectView
    {
        [Serializable]
        private class CachedInstructionInfo
        {
            public SerializedObject styleContainerSerializedObject = null;
            public SerializedProperty styleSerializedProperty = null;
            public readonly GUIStyleHolder styleContainer;

            public CachedInstructionInfo()
            {
                styleContainer = ScriptableObject.CreateInstance<GUIStyleHolder>();
            }
        }

        private Vector2 m_StacktraceScrollPos = new Vector2();

        private List<IMGUIDrawInstruction> m_Instructions = new List<IMGUIDrawInstruction>();
        private IMGUIDrawInstruction m_Instruction;
        private CachedInstructionInfo m_CachedInstructionInfo;

        private readonly List<Texture2D> m_textures = new List<Texture2D>();
        private readonly Dictionary<string, WindowStyle> m_windowStyles = new Dictionary<string, WindowStyle>();

        public void UpdateInstructions()
        {
            m_Instructions.Clear();
            GUIViewDebuggerHelper.GetDrawInstructions(m_Instructions);
            m_Instructions = m_Instructions.Where(x => !string.IsNullOrEmpty(x.label)).ToList();
        }

        public void ClearRowSelection()
        {
            m_ListViewState.row = -1;
            m_ListViewState.selectionChanged = true;
            m_CachedInstructionInfo = null;
        }

        protected void DoDrawInstruction(ListViewElement el, int id)
        {
            var listDisplayName = $"{el.row}. {m_Instructions[el.row].label}";
            var tempContent = new GUIContent(listDisplayName);

            GUIViewDebuggerWindow.Styles.listItemBackground.Draw(el.position, false, false, listViewState.row == el.row, false);

            GUIViewDebuggerWindow.Styles.listItem.Draw(el.position, tempContent, id, listViewState.row == el.row);
        }

        protected void DrawInspectedStacktrace(float availableWidth)
        {
            m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(m_StacktraceScrollPos, GUIViewDebuggerWindow.Styles.stacktraceBackground, GUILayout.ExpandHeight(false));
            DrawStackFrameList(m_Instruction.stackframes, availableWidth);
            EditorGUILayout.EndScrollView();
        }

        internal void DoDrawSelectedInstructionDetails(int selectedInstructionIndex)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CachedInstructionInfo.styleSerializedProperty, new GUIContent("Style"), true);
            if (EditorGUI.EndChangeCheck())
            {
                //m_CachedInstructionInfo.styleSerializedProperty.obj.hasModifiedProperties
                m_CachedInstructionInfo.styleContainerSerializedObject.ApplyModifiedPropertiesWithoutUndo();
                debuggerWindow.inspected.Repaint();
            }
        }

        internal string GetInstructionListName(int index)
        {
            //This means we will resolve the stack trace for all instructions.
            //TODO: make sure only visible items do this. Also, cache so we don't have to do everyframe.
            var label = m_Instructions[index].label;
            return $"{index}. {label}";
        }

        internal void OnSelectedInstructionChanged(int index)
        {
            listViewState.row = index;

            if (listViewState.row >= 0 && index < m_Instructions.Count)
            {
                if (m_CachedInstructionInfo == null)
                {
                    m_CachedInstructionInfo = new CachedInstructionInfo();
                }

                m_Instruction = m_Instructions[listViewState.row];

                //updated Cached data related to the Selected Instruction
                m_CachedInstructionInfo.styleContainer.inspectedStyle = m_Instruction.usedGUIStyle;
                m_CachedInstructionInfo.styleContainerSerializedObject = null;
                m_CachedInstructionInfo.styleSerializedProperty = null;
                GetSelectedStyleProperty(out m_CachedInstructionInfo.styleContainerSerializedObject, out m_CachedInstructionInfo.styleSerializedProperty);

                //Hightlight the item
                debuggerWindow.HighlightInstruction(debuggerWindow.inspected, m_Instruction.rect, m_Instruction.usedGUIStyle);
            }
            else
            {
                m_CachedInstructionInfo = null;

                debuggerWindow.ClearInstructionHighlighter();
            }
        }

        private void GetSelectedStyleProperty(out SerializedObject serializedObject, out SerializedProperty styleProperty)
        {
            //GUISkin[] guiskins = FindObjectsOfType<GUISkin>();
            GUISkin guiskin = null;
            //foreach (GUISkin gs in guiskins)
            var prop = typeof(GUISkin).GetField("current", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var gs = prop.GetValue(null) as GUISkin;
            GUIStyle style = gs.FindStyle(m_Instruction.usedGUIStyle.name);
            if (style != null && style == m_Instruction.usedGUIStyle)
            {
                guiskin = gs;
            }

            serializedObject = new SerializedObject(m_CachedInstructionInfo.styleContainer);
            styleProperty = serializedObject.FindProperty("inspectedStyle");
        }

        protected static class Styles
        {
            public static readonly GUIContent instructionsLabel = EditorGUIUtility.TrTextContent("Instructions");
            public static readonly GUIContent emptyViewLabel = EditorGUIUtility.TrTextContent("Select an Instruction on the left to see details");

            public static readonly GUIStyle centeredLabel = "IN CenteredLabel";
        }

        protected ListViewState listViewState => m_ListViewState;

        [NonSerialized]
        private readonly ListViewState m_ListViewState = new ListViewState();
        protected SkinEditWindow debuggerWindow => m_DebuggerWindow;
        private readonly SkinEditWindow m_DebuggerWindow;
        private Vector2 m_InstructionDetailsScrollPos = new Vector2();
        private readonly SplitterState m_InstructionDetailStacktraceSplitter = SplitterState.FromRelative(new float[] { 80, 20 }, new float[] { 100, 100 }, null);

        public BaseInspectView(SkinEditWindow guiViewDebuggerWindow)
        {
            m_DebuggerWindow = guiViewDebuggerWindow;
        }

        public void DrawInstructionList()
        {
            Event evt = Event.current;
            m_ListViewState.totalRows = m_Instructions.Count;

            EditorGUILayout.BeginVertical(GUIViewDebuggerWindow.Styles.listBackgroundStyle);
            GUILayout.Label(Styles.instructionsLabel);

            int id = GUIUtility.GetControlID(FocusType.Keyboard);
            foreach (var element in ListViewGUI.ListView(m_ListViewState, GUIViewDebuggerWindow.Styles.listBackgroundStyle))
            {
                var listViewElement = (ListViewElement)element;
                // Paint list view element
                if (evt.type == EventType.Repaint && listViewElement.row < m_Instructions.Count)
                {
                    DoDrawInstruction(listViewElement, id);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public void DrawSelectedInstructionDetails(float availableWidth)
        {
            if (m_ListViewState.selectionChanged)
                OnSelectedInstructionChanged(m_ListViewState.row);
            else if (m_ListViewState.row >= m_Instructions.Count)
                OnSelectedInstructionChanged(-1);

            if (m_CachedInstructionInfo == null)
            {
                DoDrawNothingSelected();
                return;
            }

            SplitterGUILayout.BeginVerticalSplit(m_InstructionDetailStacktraceSplitter);

            m_InstructionDetailsScrollPos = EditorGUILayout.BeginScrollView(m_InstructionDetailsScrollPos, GUIViewDebuggerWindow.Styles.boxStyle);

            DoDrawSelectedInstructionDetails(m_ListViewState.row);
            EditorGUILayout.EndScrollView();

            DrawInspectedStacktrace(availableWidth);
            SplitterGUILayout.EndVerticalSplit();
        }

        protected void DrawStackFrameList(StackFrame[] stackframes, float availableWidth)
        {
            if (stackframes != null)
            {
                var callstack = "";
                foreach (var stackframe in stackframes)
                {
                    if (string.IsNullOrEmpty(stackframe.sourceFile))
                        continue;

                    var cpos = stackframe.signature.IndexOf('(');
                    var signature = stackframe.signature.Substring(0, cpos != -1 ? cpos : stackframe.signature.Length);

                    callstack += string.Format("{0} [<a href=\"{1}\" line=\"{2}\">{1}:{2}</a>]\n", signature,
                        stackframe.sourceFile, stackframe.lineNumber);
                }

                float height = GUIViewDebuggerWindow.Styles.messageStyle.CalcHeight(new GUIContent(callstack), availableWidth);
                EditorGUILayout.SelectableLabel(callstack, GUIViewDebuggerWindow.Styles.messageStyle,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(height));
            }
        }

        protected void DrawInspectedRect(Rect instructionRect)
        {
            var totalRect = GUILayoutUtility.GetRect(0, 100);

            var reserveTopFieldHeight = Mathf.CeilToInt(EditorGUI.kSingleLineHeight * 2 + EditorGUI.kControlVerticalSpacing);
            var reserveBottomFieldHeight = Mathf.CeilToInt(EditorGUI.kSingleLineHeight);
            var reserveFieldWidth = 100;
            var fieldsArea = new RectOffset(50, reserveFieldWidth, reserveTopFieldHeight, reserveBottomFieldHeight);
            var visualRect = fieldsArea.Remove(totalRect);

            float aspectRatio = instructionRect.width / instructionRect.height;
            var aspectedRect = new Rect();
            var dummy = new Rect();

            CalculateScaledTextureRects(visualRect, ScaleMode.ScaleToFit, aspectRatio, ref aspectedRect, ref dummy);
            visualRect = aspectedRect;
            visualRect.width = Mathf.Max(80, visualRect.width);
            visualRect.height = Mathf.Max(EditorGUI.kSingleLineHeight + 10, visualRect.height);

            var startPointFieldRect = new Rect { height = EditorGUI.kSingleLineHeight, width = fieldsArea.left * 2, y = visualRect.y - fieldsArea.top };
            startPointFieldRect.x = visualRect.x - startPointFieldRect.width / 2f;

            var endPointFieldRect = new Rect
            {
                height = EditorGUI.kSingleLineHeight,
                width = fieldsArea.right * 2,
                y = visualRect.yMax
            };
            endPointFieldRect.x = visualRect.xMax - endPointFieldRect.width / 2f;

            var widthMarkersArea = new Rect
            {
                x = visualRect.x,
                y = startPointFieldRect.yMax + EditorGUI.kControlVerticalSpacing,
                width = visualRect.width,
                height = EditorGUI.kSingleLineHeight
            };

            var widthFieldRect = widthMarkersArea;
            widthFieldRect.width = widthMarkersArea.width / 3;
            widthFieldRect.x = widthMarkersArea.x + (widthMarkersArea.width - widthFieldRect.width) / 2f;

            var heightMarkerArea = visualRect;
            heightMarkerArea.x = visualRect.xMax;
            heightMarkerArea.width = EditorGUI.kSingleLineHeight;

            var heightFieldRect = heightMarkerArea;
            heightFieldRect.height = EditorGUI.kSingleLineHeight;
            heightFieldRect.width = fieldsArea.right;
            heightFieldRect.y = heightFieldRect.y + (heightMarkerArea.height - heightFieldRect.height) / 2f;

            //Draw TopLeft point
            GUI.Label(startPointFieldRect, string.Format("({0},{1})", instructionRect.x, instructionRect.y), Styles.centeredLabel);

            Handles.color = new Color(1, 1, 1, 0.5f);
            //Draw Width markers and value
            var startP = new Vector3(widthMarkersArea.x, widthFieldRect.y);
            var endP = new Vector3(widthMarkersArea.x, widthFieldRect.yMax);
            Handles.DrawLine(startP, endP);

            startP.x = endP.x = widthMarkersArea.xMax;
            Handles.DrawLine(startP, endP);

            startP.x = widthMarkersArea.x;
            startP.y = endP.y = Mathf.Lerp(startP.y, endP.y, .5f);
            endP.x = widthFieldRect.x;
            Handles.DrawLine(startP, endP);

            startP.x = widthFieldRect.xMax;
            endP.x = widthMarkersArea.xMax;
            Handles.DrawLine(startP, endP);

            GUI.Label(widthFieldRect, instructionRect.width.ToString(CultureInfo.InvariantCulture), Styles.centeredLabel);

            //Draw Height markers and value
            startP = new Vector3(heightMarkerArea.x, heightMarkerArea.y);
            endP = new Vector3(heightMarkerArea.xMax, heightMarkerArea.y);
            Handles.DrawLine(startP, endP);

            startP.y = endP.y = heightMarkerArea.yMax;
            Handles.DrawLine(startP, endP);

            startP.x = endP.x = Mathf.Lerp(startP.x, endP.x, .5f);
            startP.y = heightMarkerArea.y;
            endP.y = heightFieldRect.y;
            Handles.DrawLine(startP, endP);

            startP.y = heightFieldRect.yMax;
            endP.y = heightMarkerArea.yMax;
            Handles.DrawLine(startP, endP);

            GUI.Label(heightFieldRect, instructionRect.height.ToString(CultureInfo.InvariantCulture));

            GUI.Label(endPointFieldRect, string.Format("({0},{1})", instructionRect.xMax, instructionRect.yMax), Styles.centeredLabel);

            //Draws the rect
            GUI.Box(visualRect, GUIContent.none);
        }

        protected void DoSelectableInstructionDataField(string label, string instructionData)
        {
            var rect = EditorGUILayout.GetControlRect(true);
            EditorGUI.LabelField(rect, label);
            rect.xMin += EditorGUIUtility.labelWidth;
            EditorGUI.SelectableLabel(rect, instructionData);
        }

        private void DoDrawNothingSelected()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Styles.emptyViewLabel, GUIViewDebuggerWindow.Styles.centeredText);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        internal static bool CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, ref Rect outScreenRect, ref Rect outSourceRect)
        {
            float num = position.width / position.height;
            bool result = false;
            switch (scaleMode)
            {
                case ScaleMode.StretchToFill:
                    outScreenRect = position;
                    outSourceRect = new Rect(0f, 0f, 1f, 1f);
                    result = true;
                    break;
                case ScaleMode.ScaleAndCrop:
                    if (num > imageAspect)
                    {
                        float num4 = imageAspect / num;
                        outScreenRect = position;
                        outSourceRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
                        result = true;
                    }
                    else
                    {
                        float num5 = num / imageAspect;
                        outScreenRect = position;
                        outSourceRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
                        result = true;
                    }

                    break;
                case ScaleMode.ScaleToFit:
                    if (num > imageAspect)
                    {
                        float num2 = imageAspect / num;
                        outScreenRect = new Rect(position.xMin + position.width * (1f - num2) * 0.5f, position.yMin, num2 * position.width, position.height);
                        outSourceRect = new Rect(0f, 0f, 1f, 1f);
                        result = true;
                    }
                    else
                    {
                        float num3 = num / imageAspect;
                        outScreenRect = new Rect(position.xMin, position.yMin + position.height * (1f - num3) * 0.5f, position.width, num3 * position.height);
                        outSourceRect = new Rect(0f, 0f, 1f, 1f);
                        result = true;
                    }

                    break;
            }

            return result;
        }
    }
}
