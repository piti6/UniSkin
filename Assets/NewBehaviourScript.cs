using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Texture2D[] m_sprites;
    private void c()
    {
        ColorUtility.TryParseHtmlString("#ff756d", out var redred);
        //redred.a = 0.3f;
        ColorUtility.TryParseHtmlString("#ffcbd0", out var blue);
        blue.a = 0.3f;
        var white = Color.black;
        white.a = 0.2f;
        var grayTex = MakeTex(2, 2, white);
        var blueTex = MakeTex(2, 2, blue);
        ColorUtility.TryParseHtmlString("#fdeaca", out var orange);
        orange.a = 0.3f;
        var orangeTex = MakeTex(2, 2, orange);
        ColorUtility.TryParseHtmlString("#fdf6dc", out var green);
        green.a = 0.3f;
        var greenTex = MakeTex(2, 2, green);
        ColorUtility.TryParseHtmlString("#ccdae5", out var yellow);
        yellow.a = 0.3f;
        var yellowTex = MakeTex(2, 2, yellow);
        ColorUtility.TryParseHtmlString("#acc5e8", out var red);
        red.a = 0.3f;
        var redTex = MakeTex(2, 2, red);
        foreach (var (editorWindow, i) in Resources.FindObjectsOfTypeAll<EditorWindow>()
            .Where(editorWindow => editorWindow.titleContent.text != "Scene" && editorWindow.titleContent.text != "Game")
            .Select((x, i) => (x, i)))
        {
            Debug.LogWarning("Hoge");
            var b = editorWindow.rootVisualElement;

            var a = Color.red;
            a.a = 0.2f;

            //b.hierarchy.parent
            var styleBackground = new UnityEngine.UIElements.StyleBackground(m_sprites[0]);
            var c = b.parent[0].style;
            //c.backgroundImage = styleBackground;
            var d = b.parent[0] as IMGUIContainer;
            var current = d.onGUIHandler;

            d.onGUIHandler = () =>
            {
                var prop = typeof(GUISkin).GetField("current", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var curr = prop.GetValue(null) as GUISkin;
                //curr.scrollView.normal.background = tex;
                //curr.label.normal.background = tex;
                //curr.box.normal.background = tex;
                //curr.button.normal.background = tex;
                //curr.textField.normal.background = tex;
                //curr.textArea.normal.background = tex;
                //curr.window.normal.background = tex;
                foreach (var ccc in curr.customStyles)
                {
                    ccc.normal.background = null;
                }
                curr.label.normal.background = redTex;
                foreach (var power in curr.customStyles)
                {
                    power.normal.textColor = redred;
                    power.active.textColor = redred;
                    power.focused.textColor = redred;
                }
                //curr.customStyles.First(x => x.name.Contains("AssetLabel")).normal.background = redTex;
                //curr.customStyles.First(x => x.name.Contains("IconButton")).normal.background = redTex;
                curr.customStyles.First(x => x.name.Contains("IN BigTitle")).normal.background = grayTex;
                curr.customStyles.First(x => x.name.Contains("IN BigTitle")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("ControlLabel")).normal.background = null;
                curr.customStyles.First(x => x.name.Contains("Toolbar")).normal.background = redTex;
                curr.customStyles.First(x => x.name.Contains("dragtab first")).normal.background = redTex;
                curr.customStyles.First(x => x.name.Contains("dragtab")).normal.background = redTex;
                //curr.customStyles.First(x => x.name.Contains("ToolbarSeachTextFieldPopup")).normal.background = redTex;
                //curr.customStyles.First(x => x.name.Contains("ToolbarSeachCancelButtonEmpty")).normal.background = redTex;
                var dockHeader = curr.customStyles.First(x => x.name.Contains("dockHeader"));
                curr.customStyles.First(x => x.name.Contains("dockHeader")).normal.background = redTex;
                curr.customStyles.First(x => x.name.Contains("dockarea")).normal.background = redTex;
                curr.customStyles.First(x => x.name.Contains("SceneTopBarBg")).normal.background = blueTex;
                curr.customStyles.First(x => x.name.Contains("SceneTopBarBg")).active.background = redTex;
                curr.customStyles.First(x => x.name.Contains("ObjectField")).normal.background = grayTex;
                curr.customStyles.First(x => x.name.Contains("ObjectFieldButton")).normal.background = grayTex;
                curr.customStyles.First(x => x.name.Contains("RL Element")).normal.background = grayTex;
                curr.customStyles.First(x => x.name.Contains("TV Line")).normal.background = null;
                curr.customStyles.First(x => x.name.Contains("TV Line")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("TV LineBold")).normal.background = null;
                curr.customStyles.First(x => x.name.Contains("TV LineBold")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("IN Foldout")).normal.background = null;
                curr.customStyles.First(x => x.name.Contains("IN Foldout")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("IN Title")).normal.background = grayTex;
                curr.customStyles.First(x => x.name.Contains("IN Title")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("IN TitleText")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("Titlebar Foldout")).normal.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("Titlebar Foldout")).active.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("Titlebar Foldout")).focused.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("IN TitleText")).active.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("IN TitleText")).focused.textColor = redred;
                curr.customStyles.First(x => x.name.Contains("ScrollViewAlt")).normal.background = m_sprites[0];
                curr.customStyles.First(x => x.name.Contains("ScrollViewAlt")).active.background = redTex;
                curr.customStyles.First(x => x.name.Contains("FoldoutHeader")).normal.background = grayTex;
                curr.customStyles.First(x => x.name.Contains("TabWindowBackground")).normal.background = m_sprites[0];
                curr.customStyles.First(x => x.name.Contains("CN EntryBackOdd")).normal.background = greenTex;
                curr.customStyles.First(x => x.name.Contains("CN EntryBackEven")).normal.background = redTex;
                curr.customStyles.First(x => x.name.Contains("CN EntryWarn")).normal.textColor = redred;
                curr.textField.active.textColor = redred;
                curr.settings.selectionColor = red;
                curr.box.focused.textColor = redred;
                curr.box.active.textColor = redred;
                curr.scrollView.focused.textColor = redred;
                curr.scrollView.active.textColor = redred;
                curr.textField.focused.textColor = redred;
                curr.textArea.active.textColor = redred;
                curr.textArea.focused.textColor = redred;
                curr.label.normal.textColor = redred;
                curr.label.normal.background = redTex;
                curr.label.active.textColor = redred;
                curr.label.focused.textColor = redred;
                curr.label.active.background = redTex;
                curr.button.normal.textColor = redred;
                curr.button.normal.background = redTex;
                curr.button.active.textColor = redred;
                curr.button.focused.textColor = redred;
                curr.button.active.background = redTex;

                //UnityEngine.Debug.Log(asdf.name);
                //var ddd = curr.customStyles.First(x => x.name == "GUIStyle \\'TabWindowBackground\\'");
                //curr.customStyles = new 
                //curr.scrollView.
                //curr. = 50;

                var asm = typeof(UnityEditor.ActiveEditorTracker).Assembly;
                var hohoho = asm.GetTypes().First(x => x.Name.Contains("DockArea"));
                var stylesTypeB = hohoho.GetNestedTypes(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var stylesType = stylesTypeB.First();
                //var dockBarStyle = stylesType.GetField("dockTitleBarStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                //var dockBarGUIStyle = dockBarStyle.GetValue(null) as GUIStyle;
                //dockBarGUIStyle.normal.background = redTex;

                //var backgroundStyle = stylesType.GetField("background", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                //var backgroundGUIStyle = backgroundStyle.GetValue(null) as GUIStyle;
                //backgroundGUIStyle.normal.background = blueTex;

                var tabLabelStyle = stylesType.GetField("tabLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var tabLabelGUIStyle = tabLabelStyle.GetValue(null) as GUIStyle;
                //tabLabelGUIStyle.normal.background = blueTex;
                tabLabelGUIStyle.normal.textColor = redred;

                var dragTabStyle = stylesType.GetField("dragTab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var dragTabGUIStyle = dragTabStyle.GetValue(null) as GUIStyle;
                //dragTabGUIStyle.normal.background = blueTex;
                dragTabGUIStyle.normal.textColor = redred;

                var dragTabFirstStyle = stylesType.GetField("dragTabFirst", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var dragTabFirstGUIStyle = dragTabFirstStyle.GetValue(null) as GUIStyle;
                //dragTabFirstGUIStyle.normal.background = blueTex;
                dragTabFirstGUIStyle.normal.textColor = redred;
                //prop.SetValue(curr);
                //GUISkin.current.
                //UnityEditor.DockArea
                GUI.color = Color.blue;
                current();
                GUI.color = Color.blue;
            };
            d.ClearClassList();

            d.styleSheets.Clear();
            c.backgroundColor = new UnityEngine.UIElements.StyleColor(Color.red);
            //c.backgroundImage = styleBackground;

            //b.style.backgroundColor = white;

            c.unityBackgroundImageTintColor = white;
            c.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
        }
    }


    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            c();
        }
    }
}
