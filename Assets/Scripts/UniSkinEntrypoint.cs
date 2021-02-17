using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniSkin
{
    [InitializeOnLoad]
    public class UniSkinEntrypoint
    {
        static UniSkinEntrypoint()
        {
            EditorApplication.update += Update;
            SkinRenderer.OnWindowDispose += OnWindowDispose;
        }

        private static readonly Dictionary<string, Dictionary<int, EditorWindow>> _cachedEditorWindow = new Dictionary<string, Dictionary<int, EditorWindow>>();

        private static readonly List<int> _disposeTargetKeys = new List<int>();

        private static void Update()
        {
            foreach (var editorWindow in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                var title = editorWindow.titleContent.text;
                var instanceId = editorWindow.GetInstanceID();
                if (!_cachedEditorWindow.TryGetValue(title, out var windowDictionary))
                {
                    _cachedEditorWindow[title] = windowDictionary = new Dictionary<int, EditorWindow>();
                }

                if (!windowDictionary.TryGetValue(instanceId, out _))
                {
                    if (SkinRenderer.Register(editorWindow))
                    {
                        windowDictionary[instanceId] = editorWindow;
                    }
                }

                //Remove disposed windows
                _disposeTargetKeys.AddRange(windowDictionary.Where(x => x.Value == null).Select(x => x.Key));

                foreach (var disposeTargetKey in _disposeTargetKeys)
                {
                    windowDictionary.Remove(disposeTargetKey);
                }
            }
        }

        private static void OnWindowDispose(EditorWindow editorWindow)
        {
            var windowTitle = editorWindow.titleContent.text;
            var id = editorWindow.GetInstanceID();

            _cachedEditorWindow[windowTitle].Remove(id);
        }
    }
}
