using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniSkin
{
    [InitializeOnLoad]
    public class UniSkinEditorEntrypoint
    {
        private static readonly HashSet<string> PanelNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "Inspector",
            "Hierarchy",
            "Project",
            "Console",
        };

        private static readonly Color ProSkinColor = new Color(0.22f, 0.22f, 0.22f, 1);
        private static readonly Color FreeSkinColor = new Color(0.76f, 0.76f, 0.76f, 1);
        private static Color DefaultBackgroundColor => EditorGUIUtility.isProSkin ? ProSkinColor : FreeSkinColor;

        //private static readonly HashSet<EditorWindow>

        static UniSkinEditorEntrypoint()
        {
            _ = UniSkin.UnityEditorInternalBridge.GUIViewHelper.Hoge();
            //EditorApplication.update += Update;

            //var coreModule = Assembly.Load("UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            //var coreModule = Assembly.Load("UnityEditor.CoreModule");
            //var guiViewDebuggerHelper = coreModule.GetType("UnityEditor.GUIViewDebuggerHelper");
            //var viewMethod = guiViewDebuggerHelper.GetMethod("GetViews", BindingFlags.NonPublic | BindingFlags.Static);

            //var views = new List<GUIView>();

            //views.First().
            //viewMethod.Invoke(null, )
            //var eventInfo = guiViewDebuggerHelper.GetEvent("onViewInstructionsChanged", BindingFlags.NonPublic | BindingFlags.Static);
            //var adder = guiViewDebuggerHelper.GetMethod("add_onViewInstructionsChanged", BindingFlags.NonPublic | BindingFlags.Static);
            //GUIViewDebuggerHelper
            //Action action = () => UnityEngine.Debug.LogWarning("asdfasdklfm");
            //adder.Invoke(null, new object[] { action });

            //var asm = typeof(UnityEditor.ActiveEditorTracker).Assembly;
            //var hohoho = asm.GetTypes().First(x => x.Name.Contains("DockArea"));
            //typeof(GUIViewDebuggerWindow)
            //EditorApplication.update += Update;
        }

        private static void Update()
        {
            //UniSkin.UnityEditorInternalBridge.GUIViewHelper.Hoge();
            //foreach (var editorWindow in Resources.FindObjectsOfTypeAll<EditorWindow>().Where(x => PanelNames.Contains(x.titleContent.text)))
            //{
            //    //UnityEngine.Debug.LogWarning(editorWindow.titleContent.text);
            //}
        }
    }
}
