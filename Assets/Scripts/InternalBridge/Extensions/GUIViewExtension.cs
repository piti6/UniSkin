using UnityEditor;

namespace UniSkin
{
    internal static class GUIViewExtension
    {
        public static EditorWindow GetEditorWindow(this GUIView view) =>
            view is HostView hostView ? hostView.actualView : default;

        public static string GetViewTitleName(this GUIView view)
        {
            var editorWindow = GetEditorWindow(view);
            if (editorWindow != null)
            {
                return editorWindow.titleContent.text;
            }

            return view.GetType().Name;
        }
    }
}
