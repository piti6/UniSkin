using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniSkin.UI
{
    internal class SkinEditWindow : EditorWindow
    {
        [MenuItem("Window/UniSkin")]
        private static void ShowWindow()
        {
            GetWindow(typeof(SkinEditWindow));
        }

        private readonly GUIViewChunk _inspectedViewChunk = new GUIViewChunk();

        private readonly List<ElementHighlighter> _highlighters = new List<ElementHighlighter>();
        private readonly UniSkin.UI.InspectViewDrawer _inspectViewDrawer = new UniSkin.UI.InspectViewDrawer();

        private Skin _currentOriginalSkin;
        private MutableSkin _currentSkin;
        private bool _hasChangeOnCurrent;

        private readonly SplitterState _instructionListDetailSplitter = SplitterState.FromRelative(new float[] { 30, 70 }, new float[] { 32, 32 }, null);

        private bool CanInspectView(GUIView view) =>
            view != null && view.GetEditorWindow() != this;

        private void OnEnable()
        {
            _currentOriginalSkin = CachedSkin.Skin;
            _currentSkin = new MutableSkin(_currentOriginalSkin);

            Undo.selectionUndoRedoPerformed += UndoRedoPerformed;
            _inspectedViewChunk.OnValueChanged += OnViewChanged;
            _inspectViewDrawer.OnPropertyModify += OnPropertyModify;
            _inspectViewDrawer.OnRequestHighlight += OnRequestHighlight;
            GUIViewDebuggerHelper.onViewInstructionsChanged += OnViewInstructionsChanged;
        }

        private void OnPropertyModify(bool colorChanged, PropertyModifyData modifyData)
        {
            var currentViewName = _inspectedViewChunk.InspectedView.GetViewTitleName();
            var styleName = modifyData.ModifiedElementStyleName;
            var windowStyle = _currentSkin.WindowStyles[currentViewName];

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

            _hasChangeOnCurrent = true;

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

            _a = true;
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

        private void OnRequestHighlight(bool highlight, HighlightData highlightData)
        {
            ClearHighlighters();

            if (highlight && highlightData.View.windowBackend.visualTree is VisualElement visualElement)
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

            OnViewInstructionsChanged();
        }

        private void OnViewInstructionsChanged()
        {
            //Skip instructions changed event when Color picker window showing, since it is fairly heavy task.
            if (HasOpenInstances<ColorPicker>())
            {
                return;
            }

            _inspectViewDrawer.UpdateInstructions();
            Repaint();
        }

        private bool _a = false;
        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawCurrentSkin();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawWindowPopup();
            GUILayout.EndHorizontal();

            if (_inspectedViewChunk.InspectedView is null)
            {
                return;
            }

            DrawInstructions();
        }

        private void DrawInstructions()
        {
            SplitterGUILayout.BeginHorizontalSplit(_instructionListDetailSplitter);

            _inspectViewDrawer.DrawInstructionList();

            EditorGUILayout.BeginVertical();
            {
                var inspectedViewName = _inspectedViewChunk.InspectedView.GetViewTitleName();
                if (!_currentSkin.WindowStyles.TryGetValue(inspectedViewName, out var windowStyle))
                {
                    _currentSkin.WindowStyles[inspectedViewName] = windowStyle = new MutableWindowStyle(inspectedViewName, Array.Empty<ElementStyle>());
                }

                _inspectViewDrawer.DrawSelectedInstructionDetails(_inspectedViewChunk.InspectedView, windowStyle, _currentSkin.Textures);
            }
            EditorGUILayout.EndVertical();

            SplitterGUILayout.EndHorizontalSplit();

            EditorGUIUtility.DrawHorizontalSplitter(new Rect(_instructionListDetailSplitter.realSizes[0] + 1, EditorGUI.kWindowToolbarHeight, 1, position.height));
        }

        private void DrawCurrentSkin()
        {
            GUILayout.Label("Name:", GUILayout.ExpandWidth(false));

            var changedName = GUILayout.TextField(_currentSkin.Name, GUILayout.ExpandWidth(true));
            if (changedName != _currentSkin.Name)
            {
                _hasChangeOnCurrent = true;
                _currentSkin.Name = changedName;
            }

            var saveLabel = GUIContentUtility.UseCached("Save current skin");
            var currentSaveButtonRect = GUILayoutUtility.GetRect(saveLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUI.enabled = _hasChangeOnCurrent;
            if (GUI.Button(currentSaveButtonRect, saveLabel))
            {
                SaveCurrent();
            }
            GUI.enabled = true;

            var saveAsFileLabel = GUIContentUtility.UseCached("Save as..");
            var currentSaveAsFileButtonRect = GUILayoutUtility.GetRect(saveAsFileLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUI.Button(currentSaveAsFileButtonRect, saveAsFileLabel))
            {
                var path = EditorUtility.SaveFilePanel("Save as", string.Empty, $"{_currentSkin.Name}.skn", "skn");
                if (string.IsNullOrEmpty(path)) return;

                var json = JsonUtility.ToJson(_currentSkin.ToImmutable(grantNewId: true));

                SaveCurrent();
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

                SaveCurrent();
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

                SaveCurrent();
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

                var options = views.Where(CanInspectView)
                    .Select(x => x.GetViewTitleName())
                    .Prepend("None")
                    .Select(x => new GUIContent(x))
                    .ToArray();

                var selectedIndex = views.IndexOf(inspectedView) + 1;

                EditorUtility.DisplayCustomMenu(popupPosition, options, selectedIndex, OnInspectionValueSelected, views);
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
            Repaint();
        }

        private void OnDisable()
        {
            ClearHighlighters();
            Undo.selectionUndoRedoPerformed -= UndoRedoPerformed;
            _inspectedViewChunk.OnValueChanged -= OnViewChanged;
            _inspectViewDrawer.OnPropertyModify -= OnPropertyModify;
            _inspectViewDrawer.OnRequestHighlight -= OnRequestHighlight;
            GUIViewDebuggerHelper.onViewInstructionsChanged -= OnViewInstructionsChanged;

            if (_hasChangeOnCurrent)
            {
                if (EditorUtility.DisplayDialog("Warning", "Save unsaved chnages?", "Yes", "No"))
                {
                    SaveCurrent();
                }
                else
                {
                    Save(_currentOriginalSkin);
                }
            }
        }

        private void SaveCurrent()
        {
            Save(_currentSkin.ToImmutable(grantNewId: false));
        }

        private void Save(Skin skin)
        {
            CachedSkin.Update(skin);
            CachedSkin.Save();
            _hasChangeOnCurrent = false;

            foreach (var editorWindow in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                editorWindow.Repaint();
            }
        }
    }
}
