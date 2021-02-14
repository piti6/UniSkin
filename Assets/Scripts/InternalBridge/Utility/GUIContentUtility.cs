using System;
using UnityEngine;

public static class GUIContentUtility
{
    private static readonly Lazy<GUIContent> _cached = new Lazy<GUIContent>();
    public static GUIContent UseCached(string text)
    {
        var cached = _cached.Value;
        cached.text = text;
        return cached;
    }
}
