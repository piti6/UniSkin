using UnityEditor;
using UnityEngine.UIElements;

namespace UniSkin.UnityEditorInternalBridge
{
    internal class StylePicker
    {
        private static readonly ElementHighlighter Highlighter = new ElementHighlighter();

        internal static void Pick(GUIView view, IMGUIDrawInstruction selectedDrawInstruction)
        {
            if (view.windowBackend.visualTree is VisualElement visualElement)
            {
                Highlighter.HighlightElement(visualElement, selectedDrawInstruction.rect, selectedDrawInstruction.usedGUIStyle);
            }
        }
    }
}
