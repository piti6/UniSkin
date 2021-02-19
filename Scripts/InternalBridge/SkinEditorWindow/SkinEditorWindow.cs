using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

        private readonly StyleHighlighter _highlighter = new StyleHighlighter();

        private readonly SkinMenuView _skinMenuView = new SkinMenuView();
        private readonly InspectViewSelectView _inspectViewSelectView = new InspectViewSelectView();
        private readonly InstructionStyleView _instructionStyleView = new InstructionStyleView();

        private (GUIStyle UsedGUIStyle, IEnumerable<Rect> Rects)[] _instructionData = Array.Empty<(GUIStyle, IEnumerable<Rect>)>();
        private CachedInstructionInfo _cachedInstructionInfo;

        private Skin _currentOriginalSkin;
        private MutableSkin _currentSkin;

        private void Awake()
        {
            saveChangesMessage = "Save unsaved chnages?";
        }

        private void OnEnable()
        {
            UpdateCurrentSkin(CachedSkin.Skin);

            Undo.undoRedoPerformed += UndoRedoPerformed ;
            _inspectedViewChunk.OnViewChanged += OnViewChanged;

            _skinMenuView.OnChangeName += OnChangeName;
            _skinMenuView.OnClickLoadFromFile += OnClickLoadFromFile;
            _skinMenuView.OnClickSaveToFile += OnClickSaveToFile;
            _skinMenuView.OnClickSaveCurrent += OnClickSaveCurrent;
            _skinMenuView.OnClickRevert += OnClickRevert;
            _skinMenuView.OnClickRestoreToDefault += OnClickRestoreToDefault;

            _inspectViewSelectView.OnSelectInspectionValue += OnSelectInspectionValue;

            _instructionStyleView.OnPropertyModify += OnPropertyModify;
            _instructionStyleView.OnSelectInstruction += OnSelectInstruction;
            _instructionStyleView.OnChangeCustomBackground += OnChangeCustomBackground;

            GUIViewDebuggerHelper.onViewInstructionsChanged += OnViewInstructionsChanged;
        }

        private void OnChangeCustomBackground(bool primary, string id, SerializableTexture2D texture)
        {
            var currentWindowStyle = GetOrCreateCurrentInspectedWindowStyle();
            if (primary)
            {
                currentWindowStyle.CustomBackgroundId = id;
            }
            else
            {
                currentWindowStyle.CustomBackgroundId2 = id;
            }

            if (texture is SerializableTexture2D serializableTexture2D)
            {
                _currentSkin.Textures[id] = serializableTexture2D;
            }

            hasUnsavedChanges = true;
            UpdateCachedSkin(true);
            _inspectedViewChunk.InspectedView.Repaint();
        }

        private void OnClickRestoreToDefault()
        {
            UpdateCurrentSkin(Skin.Default);

            SaveChanges();
        }

        private void OnClickRevert()
        {
            Save(_currentOriginalSkin);
        }

        private void OnClickSaveCurrent()
        {
            SaveChanges();
        }

        private void OnClickSaveToFile(string filePath)
        {
            var json = JsonUtility.ToJson(_currentSkin.ToImmutable(grantNewId: true));

            SaveChanges();
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh();
        }

        private void OnClickLoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var skin = JsonUtility.FromJson<Skin>(json);

            UpdateCurrentSkin(skin);

            SaveChanges();
        }

        private void OnChangeName(string changedName)
        {
            hasUnsavedChanges = true;
            _currentSkin.Name = changedName;
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

                _highlighter.Highlight(true, highlightData);
            }
            else
            {
                _cachedInstructionInfo = default;

                _highlighter.Highlight(false, default);
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

        private readonly List<string> _disposeTargetKeys = new List<string>();

        private void RemoveUnusedTextures(MutableSkin targetSkin)
        {
            var customBackgroundTextureIds = targetSkin.WindowStyles.Values
                .SelectMany(x => x.CustomBackgroundIds());

            var styleBackgroundTextureIds = targetSkin.WindowStyles.Values
                .SelectMany(x => x.ElementStyles.Values)
                .SelectMany(x => x.StyleStates.Values)
                .Select(x => x.BackgroundTextureId);

            var wholeTextureIds = new HashSet<string>(Enumerable.Concat(customBackgroundTextureIds, styleBackgroundTextureIds));

            var disposableTargets = targetSkin.Textures.Keys.Where(x => !wholeTextureIds.Contains(x));

            _disposeTargetKeys.AddRange(disposableTargets);

            foreach (var disposeTargetKey in _disposeTargetKeys)
            {
                targetSkin.Textures.Remove(disposeTargetKey);
            }

            _disposeTargetKeys.Clear();
        }

        private double _lastColorChangedSeconds;
        private void UpdateCachedSkin(bool recordUndo)
        {
            if (recordUndo)
            {
                Undo.RegisterCreatedObjectUndo(CachedSkin.instance, "CachedSkin");
            }

            CachedSkin.Update(_currentSkin.ToImmutable(grantNewId: false));
        }

        private void OnViewChanged()
        {
            _highlighter.ClearHighlighters();
            _instructionStyleView.ClearRowSelection();
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

        private bool CanInspectView(GUIView view) => view != null && view.GetEditorWindow() != this;

        private void OnGUI()
        {
            _skinMenuView.Draw(_currentSkin, hasUnsavedChanges);

            _inspectViewSelectView.Draw(_inspectedViewChunk.InspectedView, CanInspectView);

            if (_inspectedViewChunk.InspectedView != null)
            {
                var inspectViewDrawerEntity = new InstructionStyleView.Entity(_instructionData, _cachedInstructionInfo, GetOrCreateCurrentInspectedWindowStyle(), _currentSkin.Textures, position.height);
                _instructionStyleView.Draw(inspectViewDrawerEntity);
            }
        }

        private MutableWindowStyle GetOrCreateCurrentInspectedWindowStyle()
        {
            var inspectedViewName = _inspectedViewChunk.InspectedView.GetViewTitleName();
            if (!_currentSkin.WindowStyles.TryGetValue(inspectedViewName, out var windowStyle))
            {
                _currentSkin.WindowStyles[inspectedViewName] = windowStyle = new MutableWindowStyle(inspectedViewName, string.Empty, string.Empty, Array.Empty<ElementStyle>());
            }

            return windowStyle;
        }

        private void OnSelectInspectionValue(object userdata, string[] options, int selected)
        {
            var selectableViews = userdata as IReadOnlyList<GUIView>;

            selected -= 1;

            _inspectedViewChunk.ChangeInspectionValue(selected >= 0 ? selectableViews[selected] : null);
        }

        private void UndoRedoPerformed()
        {
            UpdateCurrentSkin(CachedSkin.Skin);

            Repaint();

            _inspectedViewChunk.InspectedView.Repaint();
        }

        private void UpdateCurrentSkin(Skin targetSkin)
        {
            _currentOriginalSkin = targetSkin;
            _currentSkin = new MutableSkin(targetSkin);
        }

#if UNITY_2020_2_OR_NEWER
        public override void SaveChanges()
        {
            Save(_currentSkin);
        }
#else
        private void SaveChanges()
        {
            Save(_currentSkin);
        }
#endif

        private void Save(MutableSkin skin)
        {
            RemoveUnusedTextures(skin);

            Save(skin.ToImmutable(grantNewId: false));
        }

        private void Save(Skin skin)
        {
            CachedSkin.Update(skin);
            CachedSkin.Save();

            foreach (var editorWindow in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                editorWindow.Repaint();
            }

            hasUnsavedChanges = false;
        }

        private void OnDisable()
        {
            _highlighter.ClearHighlighters();

            Undo.undoRedoPerformed -= UndoRedoPerformed;
            _inspectedViewChunk.OnViewChanged -= OnViewChanged;

            _skinMenuView.OnChangeName -= OnChangeName;
            _skinMenuView.OnClickLoadFromFile -= OnClickLoadFromFile;
            _skinMenuView.OnClickSaveToFile -= OnClickSaveToFile;
            _skinMenuView.OnClickSaveCurrent -= OnClickSaveCurrent;
            _skinMenuView.OnClickRevert -= OnClickRevert;
            _skinMenuView.OnClickRestoreToDefault -= OnClickRestoreToDefault;

            _inspectViewSelectView.OnSelectInspectionValue -= OnSelectInspectionValue;

            _instructionStyleView.OnPropertyModify -= OnPropertyModify;
            _instructionStyleView.OnSelectInstruction -= OnSelectInstruction;
            _instructionStyleView.OnChangeCustomBackground -= OnChangeCustomBackground;

            GUIViewDebuggerHelper.onViewInstructionsChanged -= OnViewInstructionsChanged;

#if !UNITY_2020_2_OR_NEWER
            if (hasUnsavedChanges && EditorUtility.DisplayDialog("Warning", saveChangesMessage, "Yes", "No"))
            {
                SaveChanges();
            }
            else
            {
                Save(_currentOriginalSkin);
            }
#else
            if (hasUnsavedChanges)
            {
                Save(_currentOriginalSkin);
            }
#endif
        }
    }
}
