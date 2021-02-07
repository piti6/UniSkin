

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;

namespace UniSkin.UnityEditorInternalBridge
{
    public static class GUIViewHelper
    {
        public static async Task Hoge()
        {
            while (true)
            {
                await Task.Yield();
                //var coreModule = Assembly.Load("UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
                //var coreModule = Assembly.Load("UnityEditor.CoreModule");
                //var guiViewDebuggerHelper = coreModule.GetType("UnityEditor.GUIViewDebuggerHelper");
                //var viewMethod = guiViewDebuggerHelper.GetMethod("GetViews", BindingFlags.NonPublic | BindingFlags.Static);

                var views = new List<GUIView>();

                //views.First().
                //viewMethod.Invoke(null, )
                //var eventInfo = guiViewDebuggerHelper.GetEvent("onViewInstructionsChanged", BindingFlags.NonPublic | BindingFlags.Static);
                //var adder = guiViewDebuggerHelper.GetMethod("add_onViewInstructionsChanged", BindingFlags.NonPublic | BindingFlags.Static);

                //UnityEditor.GUIViewDebuggerHelper.GetViews(views);

                //var a = views.First(x =>
                //{
                //    return UnityEditor.StylePicker.GetEditorWindow(x)?.titleContent.text == "Inspector";
                //});
                //GUIViewDebuggerHelper.DebugWindow(a);
                //a.Repaint();

                //var instructions = new List<IMGUIDrawInstruction>();
                //GUIViewDebuggerHelper.GetDrawInstructions(instructions);

                //foreach (var inst in instructions)
                //{
                //    StylePicker.Pick(a, inst);
                //    a.Repaint();
                //    await Task.Delay(1);
                //}

                //Action action = () => UnityEngine.Debug.LogWarning("asdfasdklfm");
                //adder.Invoke(null, new object[] { action });

                //var asm = typeof(UnityEditor.ActiveEditorTracker).Assembly;
                //var hohoho = asm.GetTypes().First(x => x.Name.Contains("DockArea"));
                //typeof(GUIViewDebuggerWindow)
                //EditorApplication.update += Update;
            }
        }
    }
}
