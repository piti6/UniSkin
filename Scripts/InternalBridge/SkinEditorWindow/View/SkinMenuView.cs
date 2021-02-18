using System;
using UnityEditor;
using UnityEngine;

namespace UniSkin.UI
{
    internal class SkinMenuView
    {
        public event Action<string> OnChangeName = name => { };
        public event Action OnClickSaveCurrent = () => { };
        public event Action<string> OnClickSaveToFile = filePath => { };
        public event Action<string> OnClickLoadFromFile = filePath => { };
        public event Action OnClickRevert = () => { };
        public event Action OnClickRestoreToDefault = () => { };

        public void Draw(MutableSkin currentSkin, bool hasUnsavedChanges)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("Name:", GUILayout.ExpandWidth(false));

                var changedName = GUILayout.TextField(currentSkin.Name, GUILayout.ExpandWidth(true));
                if (changedName != currentSkin.Name)
                {
                    OnChangeName.Invoke(changedName);
                }

                GUI.enabled = hasUnsavedChanges;

                var saveLabel = GUIContentUtility.UseCached("Save current");
                if (GUI.Button(GUILayoutUtility.GetRect(saveLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)), saveLabel))
                {
                    OnClickSaveCurrent.Invoke();
                }

                GUI.enabled = true;

                var saveAsFileLabel = GUIContentUtility.UseCached("Save to file");
                var currentSaveAsFileButtonRect = GUILayoutUtility.GetRect(saveAsFileLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
                if (GUI.Button(currentSaveAsFileButtonRect, saveAsFileLabel))
                {
                    var path = EditorUtility.SaveFilePanel("Save as", string.Empty, $"{currentSkin.Name}.skn", "skn");

                    if (string.IsNullOrEmpty(path)) return;

                    OnClickSaveToFile.Invoke(path);
                }

                var loadFileLabel = GUIContentUtility.UseCached("Load from file");
                var currentLoadFileButtonRect = GUILayoutUtility.GetRect(loadFileLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
                if (GUI.Button(currentLoadFileButtonRect, loadFileLabel))
                {
                    if (hasUnsavedChanges && !EditorUtility.DisplayDialog("Warning", "Load skin from file will overwrite current skin. proceed?", "Yes", "No"))
                    {
                        return;
                    }

                    var path = EditorUtility.OpenFilePanel("Load", string.Empty, "skn");
                    if (string.IsNullOrEmpty(path)) return;

                    OnClickLoadFromFile.Invoke(path);
                }

                var revertLabel = GUIContentUtility.UseCached("Revert");
                var revertButtonRect = GUILayoutUtility.GetRect(revertLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

                GUI.enabled = hasUnsavedChanges;
                if (GUI.Button(revertButtonRect, revertLabel))
                {
                    if (!EditorUtility.DisplayDialog("Warning", "Current progress will be lost. proceed?", "Yes", "No"))
                    {
                        return;
                    }

                    OnClickRevert.Invoke();
                }
                GUI.enabled = true;

                var resetLabel = GUIContentUtility.UseCached("Restore to default");
                var resetButtonRect = GUILayoutUtility.GetRect(resetLabel, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
                if (GUI.Button(resetButtonRect, resetLabel))
                {
                    if (hasUnsavedChanges && !EditorUtility.DisplayDialog("Warning", "Current unsaved properties will be lost. proceed?", "Yes", "No"))
                    {
                        return;
                    }

                    OnClickRestoreToDefault.Invoke();
                }
            }
        }
    }
}
