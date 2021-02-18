using System;
using UnityEngine;

namespace UniSkin
{
    internal static class GUIContentUtility
    {
        private static readonly Lazy<GUIContent> _cached = new Lazy<GUIContent>();

        public static GUIContent UseCached(string text)
        {
            var cached = _cached.Value;
            cached.text = text;
            return cached;
        }
    }
}
