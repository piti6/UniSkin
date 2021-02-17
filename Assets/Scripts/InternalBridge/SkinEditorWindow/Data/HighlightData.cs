using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UniSkin.UI
{
    internal readonly struct HighlightData
    {
        public GUIView View { get; }
        public IReadOnlyList<Rect> InstructionRects { get; }
        public GUIStyle Style { get; }

        public HighlightData(GUIView view, IReadOnlyList<Rect> instructionRects, GUIStyle style)
        {
            View = view;
            InstructionRects = instructionRects;
            Style = style;
        }
    }
}
