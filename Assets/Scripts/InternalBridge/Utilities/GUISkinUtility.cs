using System;
using UnityEngine;

namespace UniSkin
{
    internal static class GUISkinUtility
    {
        private static readonly Lazy<GUISkin> _current = new Lazy<GUISkin>(() =>
        {
            var prop = typeof(GUISkin).GetField("current", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return prop.GetValue(null) as GUISkin;
        });

        public static GUISkin GetCurrent()
        {
            return _current.Value;
        }
    }
}
