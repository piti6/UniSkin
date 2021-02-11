using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniSkin
{
    [InitializeOnLoad]
    public class UniSkinEditorEntrypoint
    {
        private static readonly HashSet<string> PanelNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "Inspector",
            "Hierarchy",
            "Project",
            "Console",
        };

        private static readonly Color ProSkinColor = new Color(0.22f, 0.22f, 0.22f, 1);
        private static readonly Color FreeSkinColor = new Color(0.76f, 0.76f, 0.76f, 1);
        private static Color DefaultBackgroundColor => EditorGUIUtility.isProSkin ? ProSkinColor : FreeSkinColor;

        //private static readonly HashSet<EditorWindow>

        static UniSkinEditorEntrypoint()
        {
            EditorApplication.update += Update;

            //var coreModule = Assembly.Load("UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            //var coreModule = Assembly.Load("UnityEditor.CoreModule");
            //var guiViewDebuggerHelper = coreModule.GetType("UnityEditor.GUIViewDebuggerHelper");
            //var viewMethod = guiViewDebuggerHelper.GetMethod("GetViews", BindingFlags.NonPublic | BindingFlags.Static);

            //var views = new List<GUIView>();

            //views.First().
            //viewMethod.Invoke(null, )
            //var eventInfo = guiViewDebuggerHelper.GetEvent("onViewInstructionsChanged", BindingFlags.NonPublic | BindingFlags.Static);
            //var adder = guiViewDebuggerHelper.GetMethod("add_onViewInstructionsChanged", BindingFlags.NonPublic | BindingFlags.Static);
            //GUIViewDebuggerHelper
            //Action action = () => UnityEngine.Debug.LogWarning("asdfasdklfm");
            //adder.Invoke(null, new object[] { action });

            //var asm = typeof(UnityEditor.ActiveEditorTracker).Assembly;
            //var hohoho = asm.GetTypes().First(x => x.Name.Contains("DockArea"));
            //typeof(GUIViewDebuggerWindow)
            //EditorApplication.update += Update;
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
                    windowDictionary[instanceId] = editorWindow;
                    RegisterWindow(editorWindow);
                }

                //Remove disposed windows
                _disposeTargetKeys.AddRange(windowDictionary.Where(x => x.Value == null).Select(x => x.Key));

                foreach (var disposeTargetKey in _disposeTargetKeys)
                {
                    windowDictionary.Remove(disposeTargetKey);
                }
            }
        }

        private static void RegisterWindow(EditorWindow editorWindow)
        {
            if (!CachedSkin.Skin.WindowStyles.ContainsKey(editorWindow.titleContent.text)) return;

            var visualElement = editorWindow.rootVisualElement;

            var guiContainer = visualElement.parent[0] as IMGUIContainer;
            var originalGUIHandler = guiContainer.onGUIHandler;

            guiContainer.onGUIHandler = () =>
            {
                var skin = CachedSkin.Skin;
                var originalStyles = skin.WindowStyles[editorWindow.titleContent.text].ElementStyles.Select(x =>
                {
                    var (styleName, elementStyle) = x;
                    GUIStyle style = styleName;
                    var originalStyle = new GUIStyle(style);

                    style.fontSize = elementStyle.FontSize;
                    style.fontStyle = elementStyle.FontStyle;

                    foreach (var (stateType, state) in elementStyle.StyleStates)
                    {
                        GUIStyleState targetState = default;

                        switch (stateType)
                        {
                            case StyleStateType.Normal:
                                targetState = style.normal;
                                break;
                            case StyleStateType.Active:
                                targetState = style.active;
                                break;
                            case StyleStateType.Focused:
                                targetState = style.focused;
                                break;
                            case StyleStateType.Hover:
                                targetState = style.hover;
                                break;
                            case StyleStateType.OnNormal:
                                targetState = style.onNormal;
                                break;
                            case StyleStateType.OnActive:
                                targetState = style.onActive;
                                break;
                            case StyleStateType.OnFocused:
                                targetState = style.onFocused;
                                break;
                            case StyleStateType.OnHover:
                                targetState = style.onHover;
                                break;
                        }

                        targetState.textColor = state.TextColor;
                        targetState.scaledBackgrounds = state.ScaledBackgroundTextureIds.Where(x => x != null).Select(x => skin.Textures[x].Texture).ToArray();
                        targetState.background = skin.Textures.TryGetValue(state.BackgroundTextureId, out var serializableTexture2D) ? serializableTexture2D.Texture : null;
                    }

                    return originalStyle;
                })
                .ToArray();

                originalGUIHandler.Invoke();

                foreach (var originalStyle in originalStyles)
                {
                    GUIStyle currentStyle = originalStyle.name;
                    currentStyle.fontSize = originalStyle.fontSize;
                    currentStyle.fontStyle = originalStyle.fontStyle;
                    currentStyle.normal = originalStyle.normal;
                    currentStyle.active = originalStyle.active;
                    currentStyle.focused = originalStyle.focused;
                    currentStyle.hover = originalStyle.hover;
                    currentStyle.onNormal = originalStyle.onNormal;
                    currentStyle.onActive = originalStyle.onActive;
                    currentStyle.onFocused = originalStyle.onFocused;
                    currentStyle.onHover = originalStyle.onHover;

                    foreach (var styleState in currentStyle.AsStyleStateEnumerable().Select(x => x.StyleState))
                    {
                        if (styleState.background == null)
                        {
                            styleState.background = ColorTexture.GetDefaultColorTexture();
                        }
                    }
                }
            };
        }
    }
}
