using System.Collections.Generic;
using UnityEngine;

namespace UniSkin
{
    public static class ColorTexture
    {
        private static readonly Color ProSkinColor = new Color(0.22f, 0.22f, 0.22f, 1);
        private static readonly Color FreeSkinColor = new Color(0.76f, 0.76f, 0.76f, 1);

        private static readonly Dictionary<Color, Texture2D> _cachedTextures = new Dictionary<Color, Texture2D>();

        private static Color DefaultBackgroundColor => UnityEditor.EditorGUIUtility.isProSkin ? ProSkinColor : FreeSkinColor;

        private static Texture2D CreateColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(1, 1, color);
            texture.Apply();
            return texture;
        }

        public static Texture2D GetDefaultColorTexture()
        {
            return GetColorTexture(DefaultBackgroundColor);
        }

        public static Texture2D GetColorTexture(Color color)
        {
            if (!_cachedTextures.TryGetValue(color, out var texture))
            {
                _cachedTextures[color] = texture = CreateColorTexture(color);
            }

            return texture;
        }
    }
}
