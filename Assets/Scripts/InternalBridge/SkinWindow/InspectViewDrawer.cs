using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;

namespace UniSkin.UI
{
    internal class InspectViewDrawer
    {
        public event Action<bool, PropertyModifyData> OnPropertyModify = (colorChanged, modifyData) => { };
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
            var tempContent = GUIContentUtility.UseCached($"{el.row}. {_instructionData[el.row].UsedGUIStyle.name}");

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

        private readonly Dictionary<StyleStateType, bool> _foldOutStatus = EnumUtility.GetValues<StyleStateType>()
            .ToDictionary(x => x, _ => false);

        public void DrawSelectedInstructionDetails(GUIView currentInspectedView, MutableWindowStyle currentWindowStyle, IReadOnlyDictionary<string, SerializableTexture2D> textures)
        {
            if (_listViewState.selectionChanged)
            {
                OnSelectedInstructionChanged(_listViewState.row, currentInspectedView);
            }
            else if (_listViewState.row >= _instructionData.Count)
            {
                OnSelectedInstructionChanged(-1, currentInspectedView);
            }

            if (!_cachedInstructionInfo.IsValid)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Select an Instruction on the left to see details", GUIViewDebuggerWindow.Styles.centeredText);
                    GUILayout.FlexibleSpace();
                }

                return;
            }

            using (var propertyChangeCheckScope = new EditorGUI.ChangeCheckScope())
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_instructionDetailsScrollPos, GUIViewDebuggerWindow.Styles.boxStyle))
            {
                var inspectedStyle = _cachedInstructionInfo.StyleContainer.inspectedStyle;
                var elementStyleName = inspectedStyle.name;

                _instructionDetailsScrollPos = scrollView.scrollPosition;

                currentWindowStyle.ElementStyles.TryGetValue(elementStyleName, out var currentElementStyle);

                var beforeFontSize = currentElementStyle?.FontSize ?? inspectedStyle.fontSize;
                var afterFontSize = EditorGUILayout.IntField("FontSize: ", beforeFontSize);

                var beforeFontStyle = currentElementStyle?.FontStyle ?? inspectedStyle.fontStyle;
                var afterFontStyle = (FontStyle)EditorGUILayout.EnumFlagsField("FontStyle: ", beforeFontStyle);

                var fontHasChanged = beforeFontSize != afterFontSize || beforeFontStyle != afterFontStyle;
                if (fontHasChanged)
                {
                    var modifyData = new PropertyModifyData(elementStyleName, afterFontSize, afterFontStyle);

                    OnPropertyModify.Invoke(false, modifyData);
                }

                foreach (var styleType in EnumUtility.GetValues<StyleStateType>())
                {
                    using (var stateChangeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        MutableStyleState styleState = default;
                        currentElementStyle?.StyleStates.TryGetValue(styleType, out styleState);

                        var opened = _foldOutStatus[styleType] = EditorGUILayout.Foldout(_foldOutStatus[styleType], GUIContentUtility.UseCached(styleType.ToString()));
                        if (!opened) continue;

                        EditorGUI.indentLevel += 1;

                        styleState = styleState ?? new MutableStyleState(_cachedInstructionInfo.StyleStates[styleType]);

                        var addedTextures = new List<SerializableTexture2D>();

                        textures.TryGetValue(styleState.BackgroundTextureId ?? string.Empty, out var backgroundTexture);

                        var currentTexture = backgroundTexture?.Texture;

                        var selectedTextureObject = EditorGUILayout.ObjectField("BackgroundTexture", currentTexture, typeof(Texture2D), allowSceneObjects: false);
                        var selectedTexture = selectedTextureObject as Texture2D;
                        if (currentTexture != selectedTexture)
                        {
                            if (selectedTexture is Texture2D)
                            {
                                var serializableTexture2D = selectedTexture.ToSerializableTexture2D();
                                styleState.BackgroundTextureId = serializableTexture2D.Id;
                                addedTextures.Add(serializableTexture2D);
                            }
                            else
                            {
                                styleState.BackgroundTextureId = string.Empty;
                            }
                        }

                        GUILayout.Space(20);

                        var beforeColor = styleState.TextColor;
                        var afterColor = EditorGUILayout.ColorField(GUIContentUtility.UseCached("TextColor"), styleState.TextColor);
                        var colorHasChanged = beforeColor != afterColor;
                        styleState.TextColor = afterColor;

                        var beforeScaledBackgroundTextures = styleState.ScaledBackgroundTextureIds
                            .Select(x =>
                            {
                                textures.TryGetValue(x, out var scaledBackgroundTexture);

                                return scaledBackgroundTexture;
                            })
                            .Where(x => x is SerializableTexture2D)
                            .Select(x => x.Texture)
                            .ToArray();

                        var afterScaledBackgroundTextures = beforeScaledBackgroundTextures.ToArray();

                        GUILayout.Space(20);

                        var textureList = new ReorderableList(afterScaledBackgroundTextures, typeof(Texture2D));
                        textureList.drawHeaderCallback += rect => GUI.Label(rect, "Scaled backgrounds");
                        textureList.DoLayoutList();

                        var addedScaledBackgroundTextures = afterScaledBackgroundTextures.Except(beforeScaledBackgroundTextures).Select(x => x.ToSerializableTexture2D()).ToArray();
                        addedTextures.AddRange(addedScaledBackgroundTextures);

                        styleState.ScaledBackgroundTextureIds.AddRange(addedScaledBackgroundTextures.Select(x => x.Id));

                        if (stateChangeCheckScope.changed)
                        {
                            var modifyData = new PropertyModifyData(elementStyleName, styleState.ToImmutable(), afterFontSize, afterFontStyle, addedTextures);

                            OnPropertyModify.Invoke(colorHasChanged, modifyData);
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                }

                if (propertyChangeCheckScope.changed)
                {
                    currentInspectedView.Repaint();
                }
            }
        }
    }
}
