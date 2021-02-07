using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniSkin
{
    internal class SkinEditWindow : EditorWindow
    {
        [MenuItem("Window/UniSkin")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SkinEditWindow));
        }

        private GUIView m_Inspected;
        private EditorWindow m_InspectedEditorWindow;
        private readonly ElementHighlighter m_Highlighter = new ElementHighlighter();
        private UniSkin.UnityEditorInternalBridge.BaseInspectView m_instructionModeView;
        public void HighlightInstruction(GUIView view, Rect instructionRect, GUIStyle style)
        {
            ClearInstructionHighlighter();

            var visualElement = view.windowBackend.visualTree as UnityEngine.UIElements.VisualElement;
            if (visualElement == null)
                return;
            m_Highlighter.HighlightElement(visualElement, instructionRect, style);
        }

        public SkinEditWindow()
        {
            m_instructionModeView = new UnityEditorInternalBridge.BaseInspectView(this);
        }

        private void OnEnable()
        {
            GUIViewDebuggerHelper.onViewInstructionsChanged += OnInspectedViewChanged;
            GUIView serializedInspected = m_Inspected;
            inspected = null;
            inspected = serializedInspected;
            //m_InstructionModeView = null;
            //instructionType = m_InstructionType;
        }

        private void OnInspectedViewChanged()
        {
            m_instructionModeView.UpdateInstructions();
            Repaint();
        }

        private void OnDisable()
        {
            GUIViewDebuggerHelper.onViewInstructionsChanged -= OnInspectedViewChanged;
            inspected = null;
        }

        public GUIView inspected
        {
            get
            {
                if (m_Inspected != null || m_InspectedEditorWindow == null)
                    return m_Inspected;
                // continue inspecting the same window if its dock area is destroyed by e.g., docking or undocking it
                return inspected = m_InspectedEditorWindow.m_Parent;
            }
            private set
            {
                if (m_Inspected != value)
                {
                    ClearInstructionHighlighter();

                    m_Inspected = value;
                    if (m_Inspected != null)
                    {
                        m_InspectedEditorWindow = (m_Inspected is HostView) ? ((HostView)m_Inspected).actualView : null;
                        GUIViewDebuggerHelper.DebugWindow(m_Inspected);
                        m_Inspected.Repaint();
                    }
                    else
                    {
                        m_InspectedEditorWindow = null;
                        GUIViewDebuggerHelper.StopDebugging();
                    }
                    if (m_instructionModeView != null)
                        m_instructionModeView.ClearRowSelection();

                    OnInspectedViewChanged();
                }
            }
        }

        private readonly SplitterState m_InstructionListDetailSplitter = SplitterState.FromRelative(new float[] { 30, 70 }, new float[] { 32, 32 }, null);

        private void ShowDrawInstructions()
        {
            if (inspected == null)
            {
                ClearInstructionHighlighter();
                return;
            }

            SplitterGUILayout.BeginHorizontalSplit(m_InstructionListDetailSplitter);

            m_instructionModeView.DrawInstructionList();

            EditorGUILayout.BeginVertical();
            {
                m_instructionModeView.DrawSelectedInstructionDetails(position.width - m_InstructionListDetailSplitter.realSizes[0]);
            }
            EditorGUILayout.EndVertical();

            SplitterGUILayout.EndHorizontalSplit();

            EditorGUIUtility.DrawHorizontalSplitter(new Rect(m_InstructionListDetailSplitter.realSizes[0] + 1, EditorGUI.kWindowToolbarHeight, 1, position.height));
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            DoWindowPopup();

            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            DoToolbar();
            ShowDrawInstructions();
        }

        public void ClearInstructionHighlighter()
        {
            m_Highlighter.ClearElement();
        }

        private static EditorWindow GetEditorWindow(GUIView view)
        {
            var hostView = view as HostView;
            if (hostView != null)
                return hostView.actualView;

            return null;
        }

        private static string GetViewName(GUIView view)
        {
            var editorWindow = GetEditorWindow(view);
            if (editorWindow != null)
                return editorWindow.titleContent.text;

            return view.GetType().Name;
        }

        private bool CanInspectView(GUIView view)
        {
            if (view == null)
                return false;

            EditorWindow editorWindow = GetEditorWindow(view);
            if (editorWindow == null)
                return true;

            if (editorWindow == this)
                return false;

            return true;
        }

        private void OnWindowSelected(object userdata, string[] options, int selected)
        {
            selected--;
            inspected = selected < 0 ? null : ((List<GUIView>)userdata)[selected];
        }

        private void DoWindowPopup()
        {
            string selectedName = inspected == null ? "<Please Select>" : GetViewName(inspected);

            GUILayout.Label("Inspected View: ", GUILayout.ExpandWidth(false));

            Rect popupPosition = GUILayoutUtility.GetRect(new GUIContent(selectedName), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
            if (GUI.Button(popupPosition, new GUIContent(selectedName), EditorStyles.toolbarDropDown))
            {
                List<GUIView> views = new List<GUIView>();
                GUIViewDebuggerHelper.GetViews(views);

                List<GUIContent> options = new List<GUIContent>(views.Count + 1)
                {
                    EditorGUIUtility.TrTextContent("None")
                };

                int selectedIndex = 0;
                List<GUIView> selectableViews = new List<GUIView>(views.Count + 1);
                for (int i = 0; i < views.Count; ++i)
                {
                    GUIView view = views[i];

                    //We can't inspect ourselves, otherwise we get infinite recursion.
                    //Also avoid the InstructionOverlay
                    if (!CanInspectView(view))
                        continue;

                    GUIContent label = new GUIContent(string.Format("{0}. {1}", options.Count, GetViewName(view)));
                    options.Add(label);
                    selectableViews.Add(view);

                    if (view == inspected)
                        selectedIndex = selectableViews.Count;
                }
                //TODO: convert this to a Unity Window style popup. This way we could highlight the window on hover ;)
                EditorUtility.DisplayCustomMenu(popupPosition, options.ToArray(), selectedIndex, OnWindowSelected, selectableViews);
            }
        }
    }
}
