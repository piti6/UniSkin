using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace UniSkin.UI
{
    internal class InspectViewDrawer
    {
        public readonly struct Entity
        {
            public IReadOnlyList<(GUIStyle UsedGUIStyle, IEnumerable<Rect> Rects)> InstructionData { get; }
            public CachedInstructionInfo CachedInstructionInfo { get; }
            public MutableWindowStyle CurrentWindowStyle { get; }
            public IReadOnlyDictionary<string, SerializableTexture2D> Textures { get; }
            public float WindowHeight { get; }

            public Entity(IReadOnlyList<(GUIStyle UsedGUIStyle, IEnumerable<Rect> Rects)> instructionData,
                CachedInstructionInfo cachedInstructionInfo,
                MutableWindowStyle currentWindowStyle,
                IReadOnlyDictionary<string, SerializableTexture2D> textures,
                float windowHeight)
            {
                InstructionData = instructionData;
                CachedInstructionInfo = cachedInstructionInfo;
                CurrentWindowStyle = currentWindowStyle;
                Textures = textures;
                WindowHeight = windowHeight;
            }
        }

        public event Action<bool, PropertyModifyData> OnPropertyModify = (colorChanged, modifyData) => { };
        public event Action<bool, int> OnSelectInstruction = (selected, index) => { };

        private readonly SplitterState _instructionListDetailSplitter = new SplitterState(new float[] { 30, 70 }, new int[] { 32, 32 }, null);

        private readonly ListViewState _listViewState = new ListViewState();

        private readonly Dictionary<StyleStateType, bool> _foldOutStatus = EnumUtility.GetValues<StyleStateType>()
            .ToDictionary(x => x, _ => false);

        private static readonly string[] _backgroundModeCaptions = new string[] { "Texture", "Color" };

        private Vector2 _instructionDetailsScrollPos = Vector2.zero;

        public void Draw(Entity entity)
        {
            var rect = GUILayoutUtility.GetLastRect();
            SplitterGUILayout.BeginHorizontalSplit(_instructionListDetailSplitter);

            DrawInstructionList(entity.InstructionData);

            EditorGUILayout.BeginVertical();
            {
                DrawSelectedInstructionDetails(entity);
            }
            EditorGUILayout.EndVertical();

            SplitterGUILayout.EndHorizontalSplit();

            EditorGUIUtility.DrawHorizontalSplitter(new Rect(_instructionListDetailSplitter.realSizes[0] + 1, rect.y, 1, rect.y + entity.WindowHeight - rect.height));
        }

        private void DrawInstructionList(IReadOnlyList<(GUIStyle UsedGUIStyle, IEnumerable<Rect> Rects)> instructionData)
        {
            var evt = Event.current;
            _listViewState.totalRows = instructionData.Count;

            EditorGUILayout.BeginVertical(GUIViewDebuggerWindow.Styles.listBackgroundStyle);
            GUILayout.Label("Instructions");

            foreach (var element in ListViewGUI.ListView(_listViewState, GUIViewDebuggerWindow.Styles.listBackgroundStyle))
            {
                var listViewElement = (ListViewElement)element;
                var currentRowIndex = listViewElement.row;

                if (evt.type == EventType.Repaint && currentRowIndex < instructionData.Count)
                {
                    var isSelecting = _listViewState.row == currentRowIndex;
                    var tempContent = GUIContentUtility.UseCached($"{currentRowIndex}. {instructionData[currentRowIndex].UsedGUIStyle.name}");

                    GUIViewDebuggerWindow.Styles.listItemBackground.Draw(listViewElement.position, false, false, isSelecting, false);

                    GUIViewDebuggerWindow.Styles.listItem.Draw(listViewElement.position, tempContent, GUIUtility.GetControlID(FocusType.Keyboard), isSelecting);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedInstructionDetails(Entity entity)
        {
            if (_listViewState.selectionChanged)
            {
                OnSelectInstruction.Invoke(true, _listViewState.row);
            }
            else if (_listViewState.row >= entity.InstructionData.Count)
            {
                _listViewState.row = -1;
                OnSelectInstruction.Invoke(false, default);
            }

            if (!entity.CachedInstructionInfo.IsValid)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Select an Instruction on the left to see details", GUIViewDebuggerWindow.Styles.centeredText);
                    GUILayout.FlexibleSpace();
                }

                return;
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(_instructionDetailsScrollPos, GUIViewDebuggerWindow.Styles.boxStyle))
            {
                var inspectedStyle = entity.CachedInstructionInfo.StyleContainer.inspectedStyle;
                var elementStyleName = inspectedStyle.name;

                _instructionDetailsScrollPos = scrollView.scrollPosition;

                entity.CurrentWindowStyle.ElementStyles.TryGetValue(elementStyleName, out var currentElementStyle);

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
                        var opened = _foldOutStatus[styleType] = EditorGUILayout.Foldout(_foldOutStatus[styleType], GUIContentUtility.UseCached(styleType.ToString()));
                        if (!opened) continue;

                        EditorGUI.indentLevel += 1;

                        MutableStyleState styleState = default;
                        if (!currentElementStyle?.StyleStates.TryGetValue(styleType, out styleState) ?? true)
                        {
                            styleState = new MutableStyleState(entity.CachedInstructionInfo.StyleStates[styleType]);
                        }

                        entity.Textures.TryGetValue(styleState.BackgroundTextureId ?? string.Empty, out var backgroundTexture);

                        SerializableTexture2D addedTexture = default;
                        var beforeBackgroundColor = styleState.BackgroundColor;
                        var afterBackgroundColor = styleState.BackgroundColor;

                        var selectedMode = styleState.BackgroundType = (BackgroundType)GUILayout.SelectionGrid((int)styleState.BackgroundType, _backgroundModeCaptions, 2, "Toggle");
                        switch (selectedMode)
                        {
                            case BackgroundType.Texture:
                                {
                                    var currentTexture = backgroundTexture?.Texture;
                                    var selectedTextureObject = EditorGUILayout.ObjectField("BackgroundTexture", currentTexture, typeof(Texture2D), allowSceneObjects: false);
                                    var selectedTexture = selectedTextureObject as Texture2D;
                                    if (currentTexture != selectedTexture)
                                    {
                                        if (selectedTexture is Texture2D)
                                        {
                                            var serializableTexture2D = selectedTexture.ToSerializableTexture2D();
                                            styleState.BackgroundTextureId = serializableTexture2D.Id;
                                            addedTexture = serializableTexture2D;
                                        }
                                        else
                                        {
                                            styleState.BackgroundTextureId = string.Empty;
                                        }
                                    }
                                    break;
                                }
                            case BackgroundType.Color:
                                {
                                    styleState.BackgroundColor = afterBackgroundColor = EditorGUILayout.ColorField(GUIContentUtility.UseCached("BackgroundColor"), beforeBackgroundColor);
                                    break;
                                }

                            default: throw new Exception();
                        }

                        GUILayout.Space(20);

                        var beforeTextColor = styleState.TextColor;
                        var afterTextColor = EditorGUILayout.ColorField(GUIContentUtility.UseCached("TextColor"), beforeTextColor);
                        var colorHasChanged = beforeBackgroundColor != afterBackgroundColor || beforeTextColor != afterTextColor;
                        styleState.TextColor = afterTextColor;

                        if (stateChangeCheckScope.changed)
                        {
                            var modifyData = new PropertyModifyData(elementStyleName, styleState.ToImmutable(), addedTexture);

                            OnPropertyModify.Invoke(colorHasChanged, modifyData);
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                }
            }
        }

        public void ClearRowSelection()
        {
            _listViewState.row = -1;
            _listViewState.selectionChanged = true;
        }
    }
}
