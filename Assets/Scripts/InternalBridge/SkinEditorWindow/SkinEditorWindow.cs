using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniSkin.UI
{
    internal class SkinEditorWindow : EditorWindow
    {
#if !UNITY_2020_2_OR_NEWER
        private string saveChangesMessage = string.Empty;
        private bool hasUnsavedChanges = false;
#endif
        [MenuItem("Window/UniSkin")]
        private static void ShowWindow()
        {
            GetWindow(typeof(SkinEditorWindow), false, "SkinEditor");
        }

        private readonly GUIViewChunk _inspectedViewChunk = new GUIViewChunk();

        private readonly List<ElementHighlighter> _highlighters = new List<ElementHighlighter>();
        private readonly InspectViewDrawer _inspectViewDrawer = new InspectViewDrawer();

        private (GUIStyle UsedGUIStyle, IEnumerable<Rect> Rects)[] _instructionData = Array.Empty<(GUIStyle, IEnumerable<Rect>)>();
        private CachedInstructionInfo _cachedInstructionInfo;

        private Skin _currentOriginalSkin;
        private MutableSkin _currentSkin;

        private bool CanInspectView(GUIView view) =>
            view != null && view.GetEditorWindow() != this;

        private void OnEnable()
        {
            saveChangesMessage = "Save unsaved chnages?";
            _currentOriginalSkin = CachedSkin.Skin;
            _currentSkin = new MutableSkin(_currentOriginalSkin);

            Undo.selectionUndoRedoPerformed += UndoRedoPerformed;
            _inspectedViewChunk.OnViewChanged += OnViewChanged;
            _inspectViewDrawer.OnPropertyModify += OnPropertyModify;
            _inspectViewDrawer.OnSelectInstruction += OnSelectInstruction;
            GUIViewDebuggerHelper.onViewInstructionsChanged += OnViewInstructionsChanged;
        }

        private void UpdateInstructions()
        {
            var instructions = new List<IMGUIDrawInstruction>();
            GUIViewDebuggerHelper.GetDrawInstructions(instructions);

            _instructionData = instructions.Where(x => !string.IsNullOrEmpty(x.label))
                .GroupBy(x => x.usedGUIStyle)
                .Select(x => (x.Key, x.Select(y => y.rect)))
                .ToArray();
        }

        private void OnSelectInstruction(bool selected, int index)
        {
            var currentInspectedView = _inspectedViewChunk.InspectedView;
            if (selected)
            {
                var instruction = _instructionData[index];

                var styleContainer = CreateInstance<GUIStyleHolder>();
                styleContainer.inspectedStyle = instruction.UsedGUIStyle;

                _cachedInstructionInfo = new CachedInstructionInfo(styleContainer);

                var highlightData = new HighlightData(currentInspectedView, instruction.Rects.ToArray(), instruction.UsedGUIStyle);

                Highlight(true, highlightData);
            }
            else
            {
                _cachedInstructionInfo = default;

                Highlight(false, default);
            }
        }

        private void OnPropertyModify(bool colorChanged, PropertyModifyData modifyData)
        {
            var styleName = modifyData.ModifiedElementStyleName;
            var windowStyle = GetOrCreateCurrentInspectedWindowStyle();

            if (!windowStyle.ElementStyles.TryGetValue(styleName, out var elementStyle))
            {
                windowStyle.ElementStyles[styleName] = elementStyle = new MutableElementStyle();
            }

            elementStyle.Name = modifyData.ModifiedElementStyleName;
            elementStyle.FontSize = modifyData.FontSize;
            elementStyle.FontStyle = modifyData.FontStyle;

            if (modifyData.ModifiedStyleState is StyleState modifiedStyleState)
            {
                elementStyle.StyleStates[modifiedStyleState.StateType] = new MutableStyleState(modifiedStyleState);
            }

            // Add Target texture
            if (modifyData.AddedTexture is SerializableTexture2D addedTexture)
            {
                _currentSkin.Textures[addedTexture.Id] = addedTexture;
            }

            RemoveUnusedTextures();

            hasUnsavedChanges = true;

            var recordUndo = false;

            if (!colorChanged)
            {
                recordUndo = true;
            }
            //Throttle undo record timing as dragging on color picker palette would call Undo.RecordObject method like every frame.
            else if (EditorApplication.timeSinceStartup - _lastColorChangedSeconds > 1)
            {
                _lastColorChangedSeconds = EditorApplication.timeSinceStartup;
                recordUndo = true;
            }

            UpdateCachedSkin(recordUndo);
            _inspectedViewChunk.InspectedView?.Repaint();
        }

        private void RemoveUnusedTextures()
        {
            // Remove Unused textures
            var wholeTextureIds = _currentSkin.WindowStyles.Values
                .SelectMany(x => x.ElementStyles.Values)
                .SelectMany(x => x.StyleStates.Values)
                .Select(x => x.BackgroundTextureId)
                .Distinct();

            foreach (var textureId in wholeTextureIds.Where(x => !string.IsNullOrEmpty(x)).Where(x => !_currentSkin.Textures.ContainsKey(x)))
            {
                _currentSkin.Textures.Remove(textureId);
            }
        }

        private double _lastColorChangedSeconds;
        private void UpdateCachedSkin(bool recordUndo)
        {
            if (recordUndo)
            {
                Undo.RecordObject(CachedSkin.instance, "CachedSkin");
            }

            CachedSkin.Update(_currentSkin.ToImmutable(grantNewId: false));
        }

        private void ClearHighlighters()
        {
            foreach (var highlighter in _highlighters)
            {
                highlighter.ClearElement();
            }
        }

        private void Highlight(bool highlight, HighlightData highlightData)
        {
            ClearHighlighters();

#if UNITY_2020_2_OR_NEWER
            if (highlight && highlightData.View.windowBackend.visualTree is VisualElement visualElement)
#else
            if (highlight && highlightData.View.visualTree is VisualElement visualElement)
#endif
            {
                if (_highlighters.Count < highlightData.InstructionRects.Count)
                {
                    var newHighlighters = Enumerable.Range(0, highlightData.InstructionRects.Count - _highlighters.Count)
                        .Select(_ => new ElementHighlighter());

                    _highlighters.AddRange(newHighlighters);
                }

                foreach (var (highlighter, index) in _highlighters.Take(highlightData.InstructionRects.Count).Select((x, i) => (x, i)))
                {
                    highlighter.HighlightElement(visualElement, highlightData.InstructionRects[index], highlightData.Style);
                }
            }
        }

        private void OnViewChanged()
        {
            ClearHighlighters();
            _inspectViewDrawer.ClearRowSelection();
            _cachedInstructionInfo = default;

            OnViewInstructionsChanged();
        }

        private void OnViewInstructionsChanged()
        {
            ///Skip instructions changed event when Color picker window showing, since it is fairly heavy task.
            ///(dragging on color picker palette fires tons of <see cref="GUIViewDebuggerHelper.onViewInstructionsChanged"/> event)
            if (HasOpenInstances<ColorPicker>())
            {
                return;
            }

            UpdateInstructions();
            Repaint();
        }

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                DrawCurrentSkin();
            }

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                DrawWindowPopup();
            }

            if (_inspectedViewChunk.InspectedView is null)
            {
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawCurrentInspectViewBackgroundTexture();
            }

            var inspectViewDrawerEntity = new InspectViewDrawer.Entity(_instructionData, _cachedInstructionInfo, GetOrCreateCurrentInspectedWindowStyle(), _currentSkin.Textures, position.height);
            _inspectViewDrawer.Draw(inspectViewDrawerEntity);
        }

        private MutableWindowStyle GetOrCreateCurrentInspectedWindowStyle()
        {
            var inspectedViewName = _inspectedViewChunk.InspectedView.GetViewTitleName();
            if (!_currentSkin.WindowStyles.TryGetValue(inspectedViewName, out var windowStyle))
            {
                _currentSkin.WindowStyles[inspectedViewName] = windowStyle = new MutableWindowStyle(inspectedViewName, string.Empty, Array.Empty<ElementStyle>());
            }

            return windowStyle;
        }

        private void DrawCurrentSkin()
        {
            GUILayout.Label("Name:", GUILayout.ExpandWidth(false));

            var changedName = GUILayout.TextField(_currentSkin.Name, GUILayout.ExpandWidth(true));
            if (changedName != _currentSkin.Name)
            {
                hasUnsavedChanges = true;
                _currentSkin.Name = changedName;
            }

            var saveLabel = GUIContentUtility.UseCached("Save current skin");
            var currentSaveButtonRect = GUILayoutUtility.GetRect(saveLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUI.enabled = hasUnsavedChanges;
            if (GUI.Button(currentSaveButtonRect, saveLabel))
            {
                SaveChanges();
            }
            GUI.enabled = true;

            var saveAsFileLabel = GUIContentUtility.UseCached("Save as..");
            var currentSaveAsFileButtonRect = GUILayoutUtility.GetRect(saveAsFileLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUI.Button(currentSaveAsFileButtonRect, saveAsFileLabel))
            {
                var path = EditorUtility.SaveFilePanel("Save as", string.Empty, $"{_currentSkin.Name}.skn", "skn");
                if (string.IsNullOrEmpty(path)) return;

                var json = JsonUtility.ToJson(_currentSkin.ToImmutable(grantNewId: true));

                SaveChanges();
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
            }

            var loadFileLabel = GUIContentUtility.UseCached("Load file");
            var currentLoadFileButtonRect = GUILayoutUtility.GetRect(loadFileLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUI.Button(currentLoadFileButtonRect, loadFileLabel))
            {
                if (!EditorUtility.DisplayDialog("Warning", "Load skin file will overwrite current skin when current skin not saved as file. proceed?", "Yes", "No"))
                {
                    return;
                }

                var path = EditorUtility.OpenFilePanel("Load", string.Empty, "skn");
                if (string.IsNullOrEmpty(path)) return;

                var json = File.ReadAllText(path);
                var skin = JsonUtility.FromJson<Skin>(json);

                _currentOriginalSkin = skin;
                _currentSkin = new MutableSkin(skin);

                SaveChanges();
            }

            var resetLabel = GUIContentUtility.UseCached("Reset to default");
            var resetButtonRect = GUILayoutUtility.GetRect(resetLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUI.Button(resetButtonRect, resetLabel))
            {
                if (!EditorUtility.DisplayDialog("Warning", "Current unsaved properties will be lost. proceed?", "Yes", "No"))
                {
                    return;
                }

                _currentOriginalSkin = Skin.Default;
                _currentSkin = new MutableSkin(_currentOriginalSkin);

                SaveChanges();
            }
        }

        private void DrawWindowPopup()
        {
            var inspectedView = _inspectedViewChunk.InspectedView;
            var selectedName = inspectedView is GUIView ? inspectedView.GetViewTitleName() : "<Please Select>";
            var selectedNameLabel = GUIContentUtility.UseCached(selectedName);

            GUILayout.Label("Inspected View: ", GUILayout.ExpandWidth(false));

            var popupPosition = GUILayoutUtility.GetRect(selectedNameLabel, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
            if (GUI.Button(popupPosition, selectedNameLabel, EditorStyles.toolbarDropDown))
            {
                var views = new List<GUIView>();
                GUIViewDebuggerHelper.GetViews(views);
                views = views.Where(CanInspectView).ToList();

                var options = views
                    .Select(x => x.GetViewTitleName())
                    .Prepend("None")
                    .Select(x => new GUIContent(x))
                    .ToArray();

                var selectedIndex = views.IndexOf(inspectedView) + 1;

                EditorUtility.DisplayCustomMenu(popupPosition, options, selectedIndex, OnInspectionValueSelected, views);
            }
        }

        private void DrawCurrentInspectViewBackgroundTexture()
        {
            var windowStyle = GetOrCreateCurrentInspectedWindowStyle();
            _currentSkin.Textures.TryGetValue(windowStyle.CustomBackgroundId ?? string.Empty, out var customBackgroundTexture);

            var currentCustomBackgroundTexture = customBackgroundTexture?.Texture;
            var selectedCustomBackgroundTextureObject = EditorGUILayout.ObjectField("BackgroundTexture", currentCustomBackgroundTexture, typeof(Texture2D), allowSceneObjects: false, GUILayout.ExpandWidth(true));
            var selectedCustomBackgroundTexture = selectedCustomBackgroundTextureObject as Texture2D;
            if (currentCustomBackgroundTexture != selectedCustomBackgroundTexture)
            {
                if (selectedCustomBackgroundTexture is Texture2D)
                {
                    var serializableTexture2D = selectedCustomBackgroundTexture.ToSerializableTexture2D();
                    windowStyle.CustomBackgroundId = serializableTexture2D.Id;

                    _currentSkin.Textures[serializableTexture2D.Id] = serializableTexture2D;
                }
                else
                {
                    windowStyle.CustomBackgroundId = default;
                }

                hasUnsavedChanges = true;
                RemoveUnusedTextures();
                UpdateCachedSkin(true);
                _inspectedViewChunk.InspectedView.Repaint();
            }
        }

        private void OnInspectionValueSelected(object userdata, string[] options, int selected)
        {
            var selectableViews = userdata as IReadOnlyList<GUIView>;

            selected -= 1;

            _inspectedViewChunk.ChangeInspectionValue(selected >= 0 ? selectableViews[selected] : null);
        }

        private void UndoRedoPerformed(Undo.UndoRedoType obj)
        {
            _currentOriginalSkin = CachedSkin.Skin;
            _currentSkin = new MutableSkin(_currentOriginalSkin);

            Repaint();
        }

        private void OnDisable()
        {
            ClearHighlighters();
            Undo.selectionUndoRedoPerformed -= UndoRedoPerformed;
            _inspectedViewChunk.OnViewChanged -= OnViewChanged;
            _inspectViewDrawer.OnPropertyModify -= OnPropertyModify;
            _inspectViewDrawer.OnSelectInstruction -= OnSelectInstruction;
            GUIViewDebuggerHelper.onViewInstructionsChanged -= OnViewInstructionsChanged;

            if (hasUnsavedChanges)
            {
                Save(_currentOriginalSkin);
            }
        }

#if UNITY_2020_2_OR_NEWER
        public override void SaveChanges()
        {
            Save(_currentSkin.ToImmutable(grantNewId: false));

            base.SaveChanges();
        }
#else
        public void SaveChanges()
        {
            Save(_currentSkin.ToImmutable(grantNewId: false));

            hasUnsavedChanges = false;
        }
#endif

        private void Save(Skin skin)
        {
            CachedSkin.Update(skin);
            CachedSkin.Save();

            foreach (var editorWindow in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                editorWindow.Repaint();
            }
        }
    }
}
