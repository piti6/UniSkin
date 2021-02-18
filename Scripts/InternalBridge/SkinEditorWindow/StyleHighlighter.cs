using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace UniSkin
{
    internal class StyleHighlighter
    {
        private readonly List<ElementHighlighter> _highlighters = new List<ElementHighlighter>();

        public void ClearHighlighters()
        {
            foreach (var highlighter in _highlighters)
            {
                highlighter.ClearElement();
            }
        }

        public void Highlight(bool highlight, UI.HighlightData highlightData)
        {
            ClearHighlighters();

#if UNITY_2020_2_OR_NEWER
            if (highlight && highlightData.View.windowBackend.visualTree is VisualElement visualElement)
#else
            if (highlight && highlightData.View.visualTree is VisualElement visualElement)
#endif
            {
                if (_highlighters.Count < highlightData.InstructionRects.Count)
                {
                    var newHighlighters = Enumerable.Range(0, highlightData.InstructionRects.Count - _highlighters.Count)
                        .Select(_ => new ElementHighlighter());

                    _highlighters.AddRange(newHighlighters);
                }

                foreach (var (highlighter, index) in _highlighters.Take(highlightData.InstructionRects.Count).Select((x, i) => (x, i)))
                {
                    highlighter.HighlightElement(visualElement, highlightData.InstructionRects[index], highlightData.Style);
                }
            }
        }
    }
}
