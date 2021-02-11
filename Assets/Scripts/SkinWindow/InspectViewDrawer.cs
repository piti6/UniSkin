using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace UniSkin.UI
{
    internal class InspectViewDrawer
    {
        public event Action<PropertyModifyData> OnPropertyModify = modifyData => { };
        public event Action<bool, HighlightData> OnRequestHighlight = (needHighlight, highlightData) => { };

        private List<(GUIStyle UsedGUIStyle, IEnumerable<Rect> Rects)> _instructionData = new List<(GUIStyle UsedGUIStyle, IEnumerable<Rect>)>();
        private CachedInstructionInfo _cachedInstructionInfo;

        [NonSerialized]
        private readonly ListViewState _listViewState = new ListViewState();
        private Vector2 _instructionDetailsScrollPos = new Vector2();

        public void UpdateInstructions()
        {
            var instructions = new List<IMGUIDrawInstruction>();
            GUIViewDebuggerHelper.GetDrawInstructions(instructions);

            _instructionData = instructions.Where(x => !string.IsNullOrEmpty(x.label))
                .GroupBy(x => x.usedGUIStyle)
                .Select(x => (x.Key, x.Select(y => y.rect)))
                .ToList();
        }

        public void ClearRowSelection()
        {
            _listViewState.row = -1;
            _listViewState.selectionChanged = true;
            _cachedInstructionInfo = default;
        }

        protected void DoDrawInstruction(ListViewElement el, int id)
        {
            var listDisplayName = $"{el.row}. {_instructionData[el.row].UsedGUIStyle.name}";
            var tempContent = new GUIContent(listDisplayName);

            GUIViewDebuggerWindow.Styles.listItemBackground.Draw(el.position, false, false, _listViewState.row == el.row, false);

            GUIViewDebuggerWindow.Styles.listItem.Draw(el.position, tempContent, id, _listViewState.row == el.row);
        }

        internal void OnSelectedInstructionChanged(int index, GUIView currentInspectedView)
        {
            _listViewState.row = index;

            if (_listViewState.row >= 0 && index < _instructionData.Count)
            {
                var instruction = _instructionData[_listViewState.row];

                var styleContainer = ScriptableObject.CreateInstance<GUIStyleHolder>();
                styleContainer.inspectedStyle = instruction.UsedGUIStyle;

                _cachedInstructionInfo = new CachedInstructionInfo(styleContainer);

                var highlightData = new HighlightData(currentInspectedView, instruction.Rects.ToArray(), instruction.UsedGUIStyle);

                OnRequestHighlight.Invoke(true, highlightData);
            }
            else
            {
                _cachedInstructionInfo = default;

                OnRequestHighlight.Invoke(false, default);
            }
        }

        public void DrawInstructionList()
        {
            Event evt = Event.current;
            _listViewState.totalRows = _instructionData.Count;

            EditorGUILayout.BeginVertical(GUIViewDebuggerWindow.Styles.listBackgroundStyle);
            GUILayout.Label("Instructions");

            int id = GUIUtility.GetControlID(FocusType.Keyboard);
            foreach (var element in ListViewGUI.ListView(_listViewState, GUIViewDebuggerWindow.Styles.listBackgroundStyle))
            {
                var listViewElement = (ListViewElement)element;
                // Paint list view element
                if (evt.type == EventType.Repaint && listViewElement.row < _instructionData.Count)
                {
                    DoDrawInstruction(listViewElement, id);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public void DrawSelectedInstructionDetails(GUIView currentInspectedView)
        {
            if (_listViewState.selectionChanged)
            {
                OnSelectedInstructionChanged(_listViewState.row, currentInspectedView);
            }
            else if (_listViewState.row >= _instructionData.Count)
            {
                OnSelectedInstructionChanged(-1, currentInspectedView);
            }

            if (_cachedInstructionInfo.IsValid)
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_instructionDetailsScrollPos, GUIViewDebuggerWindow.Styles.boxStyle))
                {
                    _instructionDetailsScrollPos = scrollView.scrollPosition;
                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        foreach (var kv in _cachedInstructionInfo.StyleStateProperty)
                        {
                            using (var propertyChangeCheckScope = new EditorGUI.ChangeCheckScope())
                            {
                                EditorGUILayout.PropertyField(kv.Value, new GUIContent(kv.Key.ToString()), true);

                                if (propertyChangeCheckScope.changed)
                                {
                                    var elementStyleName = _cachedInstructionInfo.StyleContainer.inspectedStyle.name;
                                    var guiStyleState = kv.Value.GetObject() as GUIStyleState;

                                    _cachedInstructionInfo.StyleContainerSerializedObject.ApplyModifiedProperties();

                                    var textures = guiStyleState.scaledBackgrounds.Append(guiStyleState.background).Where(x => x != null);

                                    var modifyData = new PropertyModifyData(elementStyleName, guiStyleState.ToStyleState(kv.Key), textures);

                                    var group = Undo.GetCurrentGroup();

                                    OnPropertyModify.Invoke(modifyData);

                                    Undo.CollapseUndoOperations(group);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Select an Instruction on the left to see details", GUIViewDebuggerWindow.Styles.centeredText);
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}
