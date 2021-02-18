using System;
using UnityEditor;

namespace UniSkin.UI
{
    internal class GUIViewChunk
    {
        public event Action OnViewChanged = () => { };

        public GUIView InspectedView
        {
            get
            {
                if (m_inspectedView != null)
                {
                    return m_inspectedView;
                }
                else
                {
                    ChangeInspectionValue(m_inspectedEditorWindow?.m_Parent);

                    return m_inspectedView;
                }
            }
        }

        private GUIView m_inspectedView;
        private EditorWindow m_inspectedEditorWindow;

        public GUIViewChunk()
        {
            m_inspectedView = default;
            m_inspectedEditorWindow = default;
        }

        public void ChangeInspectionValue(GUIView value)
        {
            if (m_inspectedView == value) return;

            if (value is GUIView)
            {
                GUIViewDebuggerHelper.DebugWindow(value);
                value.Repaint();

                var hostView = value as HostView;

                m_inspectedView = value;
                m_inspectedEditorWindow = hostView?.actualView;
            }
            else
            {
                GUIViewDebuggerHelper.StopDebugging();

                m_inspectedView = null;
                m_inspectedEditorWindow = null;
            }

            OnViewChanged.Invoke();
        }
    }
}
