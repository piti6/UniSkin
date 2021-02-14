using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniSkin
{
    public static class SkinRenderer
    {
        private static readonly HashSet<string> PanelNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "Inspector",
            "Hierarchy",
            "Project",
            "Console",
        };

        public static Action<EditorWindow> OnWindowDispose = editorWindow => { };

        private static readonly Dictionary<string, GUIStyle> _cachedOriginalStyles = new Dictionary<string, GUIStyle>();

        public static bool Register(EditorWindow editorWindow)
        {
            var title = editorWindow.titleContent.text;
            if (!CachedSkin.Skin.WindowStyles.ContainsKey(title))
            {
                return false;
            }

            var visualElement = editorWindow.rootVisualElement;

            var guiContainer = visualElement.parent[0] as IMGUIContainer;
            var originalGUIHandler = guiContainer.onGUIHandler;

            guiContainer.onGUIHandler = () =>
            {
                var skin = CachedSkin.Skin;
                if (!skin.WindowStyles.TryGetValue(title, out var windowStyle))
                {
                    guiContainer.onGUIHandler = originalGUIHandler;
                    editorWindow.Repaint();

                    OnWindowDispose.Invoke(editorWindow);
                    return;
                }

                var originalStyles = windowStyle.ElementStyles
                    .Select(x =>
                    {
                        var (styleName, elementStyle) = x;
                        var currentSkin = GUISkinUtility.GetCurrent();
                        var style = currentSkin.customStyles.FirstOrDefault(x => x.name == styleName);
                        if (style is null)
                        {
                            style = currentSkin.FindStyle(styleName);
                            if (style is null)
                            {
                                return null;
                            }
                        }

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
                            targetState.scaledBackgrounds = state.ScaledBackgroundTextureIds.Where(x => !string.IsNullOrEmpty(x)).Select(x => skin.Textures[x].Texture).ToArray();
                            targetState.background = !string.IsNullOrEmpty(state.BackgroundTextureId) && skin.Textures.TryGetValue(state.BackgroundTextureId, out var serializableTexture2D) ?
                                serializableTexture2D.Texture : null;
                        }

                        return originalStyle;
                    })
                    .OfType<GUIStyle>()
                    .ToArray();

                originalGUIHandler.Invoke();

                foreach (var originalStyle in originalStyles)
                {
                    GUIStyle currentStyle = originalStyle.name;
                    currentStyle.ApplyUniSkinStyle(originalStyle);
                }
            };

            return true;
        }
    }
}
