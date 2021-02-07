using UnityEditor;
using UnityEngine.UIElements;

namespace UniSkin
{
    public class UniSkinSettingsProvider : SettingsProvider
    {
        public UniSkinSettingsProvider(string path)
            : base(path, SettingsScope.User)
        {
        }

        public override bool HasSearchInterest(string searchContext)
        {
            return base.HasSearchInterest(searchContext);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public override void OnFooterBarGUI()
        {
            base.OnFooterBarGUI();
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
        }

        public override void OnInspectorUpdate()
        {
            base.OnInspectorUpdate();
        }

        public override void OnTitleBarGUI()
        {
            base.OnTitleBarGUI();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
