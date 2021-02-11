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
        private readonly UniSkin.UI.InspectViewDrawer _inspectView = new UniSkin.UI.InspectViewDrawer();

        private MutableSkin _currentSkin;
        private bool _hasChangeOnCurrent;

        private List<Skin> _cachedSkins;

        private readonly SplitterState _instructionListDetailSplitter = SplitterState.FromRelative(new float[] { 30, 70 }, new float[] { 32, 32 }, null);

        private static EditorWindow GetEditorWindow(GUIView view) =>
            view is HostView hostView ? hostView.actualView : default;

        private bool CanInspectView(GUIView view) =>
            view != null && GetEditorWindow(view) != this;

        private void OnEnable()
        {
            _currentSkin = new MutableSkin(CachedSkin.Skin);

            Undo.selectionUndoRedoPerformed += UndoRedoPerformed;
            _inspectedViewChunk.OnValueChanged += OnViewChanged;
            _inspectView.OnPropertyModify += OnPropertyModify;
            _inspectView.OnRequestHighlight += OnRequestHighlight;
            GUIViewDebuggerHelper.onViewInstructionsChanged += OnInspectedViewChanged;
        }

        private void OnPropertyModify(PropertyModifyData modifyData)
        {
            Undo.RecordObject(CachedSkin.instance, "CachedSkin");
            var currentViewName = GetViewName(_inspectedViewChunk.InspectedView);
            var styleName = modifyData.ModifiedElementStyleName;
            if (!_currentSkin.WindowStyles.TryGetValue(currentViewName, out var windowStyle))
            {
                _currentSkin.WindowStyles[currentViewName] = windowStyle = new MutableWindowStyle(currentViewName, new ElementStyle[] { });
            }

            GUIStyle guiStyle = styleName;
            var newElementStyle = new MutableElementStyle(modifyData.ModifiedElementStyleName, guiStyle.fontSize, guiStyle.fontStyle, new StyleState[] { });

            if (!windowStyle.ElementStyles.TryGetValue(styleName, out var elementStyle))
            {
                windowStyle.ElementStyles[styleName] = elementStyle = new MutableElementStyle();
            }

            elementStyle.Name = modifyData.ModifiedElementStyleName;
            elementStyle.FontSize = guiStyle.fontSize;
            elementStyle.FontStyle = guiStyle.fontStyle;
            elementStyle.StyleStates[modifyData.ModifiedStyleState.StateType] = new MutableStyleState(modifyData.ModifiedStyleState);

            var targetTextures = modifyData.AddedTextures.Where(x => !_currentSkin.Textures.ContainsKey(x.Id));
            foreach (var targetTexture in targetTextures)
            {
                _currentSkin.Textures[targetTexture.Id] = targetTexture;
            }

            _hasChangeOnCurrent = true;
            Reserialize();
        }

        private void Reserialize()
        {
            CachedSkin.Update(_currentSkin.ToImmutable());
            CachedSkin.Save();
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
            _inspectView.ClearRowSelection();

            OnInspectedViewChanged();
        }

        private void OnInspectedViewChanged()
        {
            _inspectView.UpdateInstructions();
            Repaint();
        }

        private void ShowDrawInstructions()
        {
            SplitterGUILayout.BeginHorizontalSplit(_instructionListDetailSplitter);

            _inspectView.DrawInstructionList();

            EditorGUILayout.BeginVertical();
            {
                _inspectView.DrawSelectedInstructionDetails(_inspectedViewChunk.InspectedView);
            }
            EditorGUILayout.EndVertical();

            SplitterGUILayout.EndHorizontalSplit();

            EditorGUIUtility.DrawHorizontalSplitter(new Rect(_instructionListDetailSplitter.realSizes[0] + 1, EditorGUI.kWindowToolbarHeight, 1, position.height));
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DoWindowPopup();
            GUILayout.EndHorizontal();

            if (_inspectedViewChunk.InspectedView is null)
            {
                return;
            }

            ShowDrawInstructions();
        }

        private static string GetViewName(GUIView view)
        {
            var editorWindow = GetEditorWindow(view);
            if (editorWindow != null)
            {
                return editorWindow.titleContent.text;
            }

            return view.GetType().Name;
        }

        private void DoWindowPopup()
        {
            var inspectedView = _inspectedViewChunk.InspectedView;
            var selectedName = inspectedView is GUIView ? GetViewName(inspectedView) : "<Please Select>";

            GUILayout.Label("Inspected View: ", GUILayout.ExpandWidth(false));

            var popupPosition = GUILayoutUtility.GetRect(new GUIContent(selectedName), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
            if (UnityEngine.GUI.Button(popupPosition, new GUIContent(selectedName), EditorStyles.toolbarDropDown))
            {
                var views = new List<GUIView>();
                GUIViewDebuggerHelper.GetViews(views);
                views = views.Where(CanInspectView).ToList();

                var options = views.Where(CanInspectView)
                    .Select(GetViewName)
                    .Prepend("None")
                    .Select(x => new GUIContent(x))
                    .ToArray();

                var selectedIndex = views.IndexOf(inspectedView) + 1;

                EditorUtility.DisplayCustomMenu(popupPosition, options, selectedIndex, OnInspectionValueSelected, views);
            }

            GUILayout.Label("Current selected skin: ", GUILayout.ExpandWidth(false));

            var currentSkinName = new GUIContent(_currentSkin.Name);
            var currentSkinPopupPosition = GUILayoutUtility.GetRect(currentSkinName, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
            if (UnityEngine.GUI.Button(currentSkinPopupPosition, currentSkinName, EditorStyles.toolbarDropDown))
            {
                if (_cachedSkins == default)
                {
                    var projectPath = System.IO.Directory.GetCurrentDirectory();

                    _cachedSkins = Directory.GetFiles(Application.dataPath, "*.skn", SearchOption.AllDirectories)
                        .Where(x => !x.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Replace(projectPath, string.Empty))
                        .Select(AssetDatabase.LoadAssetAtPath<TextAsset>)
                        .Select(x => JsonUtility.FromJson<Skin>(x.text))
                        .Prepend(CachedSkin.Skin)
                        .ToList();

                    //Todo: Package preset
                }

                var options = _cachedSkins
                    .Select(x => x.Name)
                    .Select(x => new GUIContent(x))
                    .ToArray();

                var selectedSkin = _cachedSkins.Select((cachedSkin, index) => (cachedSkin, index)).FirstOrDefault(x => x.cachedSkin.Id == _currentSkin.Id);
                var selectedIndex = selectedSkin.cachedSkin is Skin ? selectedSkin.index : -1;

                EditorUtility.DisplayCustomMenu(currentSkinPopupPosition, options, selectedIndex, OnSkinSelected, _cachedSkins);
            }

            var currentButtonRect = GUILayoutUtility.GetRect(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            GUI.Button(currentButtonRect, new GUIContent("Save"));
        }

        private void OnInspectionValueSelected(object userdata, string[] options, int selected)
        {
            var selectableViews = userdata as IReadOnlyList<GUIView>;

            selected -= 1;

            _inspectedViewChunk.ChangeInspectionValue(selected >= 0 ? selectableViews[selected] : null);
        }

        private void OnSkinSelected(object userdata, string[] options, int selected)
        {
            if (_currentSkin is MutableSkin && _hasChangeOnCurrent)
            {
                if (!EditorUtility.DisplayDialog("Warning", "Load another skin will lost current unsaved changes, proceed?", "Yes", "No"))
                {
                    return;
                }
            }

            var selectableSkins = userdata as IReadOnlyList<Skin>;

            _currentSkin = new MutableSkin(selectableSkins[selected]);
        }

        private void UndoRedoPerformed(Undo.UndoRedoType obj)
        {
            CachedSkin.Save();
            Repaint();
        }

        private void OnDisable()
        {
            ClearHighlighters();
            Undo.selectionUndoRedoPerformed -= UndoRedoPerformed;
            _inspectedViewChunk.OnValueChanged -= OnViewChanged;
            _inspectView.OnPropertyModify -= OnPropertyModify;
            _inspectView.OnRequestHighlight -= OnRequestHighlight;
            GUIViewDebuggerHelper.onViewInstructionsChanged -= OnInspectedViewChanged;
        }
    }
}
