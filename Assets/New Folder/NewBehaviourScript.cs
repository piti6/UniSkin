using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace UniSkin
{
    public class NewBehaviourScript : MonoBehaviour
    {
        private static Color GetDefaultBackgroundColor()
        {
            float kViewBackgroundIntensity = EditorGUIUtility.isProSkin ? 0.22f : 0.76f;
            return new Color(kViewBackgroundIntensity, kViewBackgroundIntensity, kViewBackgroundIntensity, 1f);
        }

        private bool did = false;
        [SerializeField] private Texture2D[] m_sprites;
        [SerializeField] private GUISkin skin;
        internal GUIStyle GetStyle(string styleName)
        {
            GUIStyle s = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (s == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
            }
            return s;
        }

        //private SerializableTexture2D power;
        private void Start()
        {
            //var serializableTexture2D = new SerializableTexture2D();
            //var json = JsonUtility.ToJson(serializableTexture2D);

            //power = JsonUtility.FromJson<SerializableTexture2D>(json);

            //var watch = new Stopwatch();
            //watch.Start();
            //Resources.FindObjectsOfTypeAll<EditorWindow>();

            //watch.Stop();

            //UnityEngine.Debug.LogWarning(watch.Elapsed);
            ////EventInfo eventInfo = typeof(ActiveEditorTracker).GetEvent("editorTrackerRebuilt",
            ////    BindingFlags.NonPublic | BindingFlags.Static);
            ////MethodInfo adder = typeof(ActiveEditorTracker).GetMethod("add_editorTrackerRebuilt",
            ////    BindingFlags.NonPublic | BindingFlags.Static);

            ////Action action = () => UnityEngine.Debug.LogWarning("asdfasdklfm");
            ////adder.Invoke(null, new object[] { action });

            //var fieldInfo = typeof(EditorApplication).GetField("windowsReordered", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            //var del = fieldInfo.GetValue(null) as EditorApplication.CallbackFunction;
            //del += () => Debug.LogWarning("l213412rio23ur");

            //fieldInfo.SetValue(null, del);

            //var eventInfo = typeof(ActiveEditorTracker).GetEvent("editorTrackerRebuilt", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            //eventInfo.AddEventHandler(null, new EventHandler((_, __) => UnityEngine.Debug.LogWarning("asdfasdklfm")));
        }

        private void c()
        {
            var prop = typeof(GUISkin).GetField("current", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var curr = prop.GetValue(null) as GUISkin;
            curr.settings.cursorColor = Color.green;
            curr.settings.selectionColor = Color.red;
            //AssetDatabase.CreateAsset(curr, "New GUISkin.guiskin");
            //AssetDatabase.SaveAssets();
            //var a = EditorWindow.m_Parent;

            var defaultColor = MakeTex(1, 1, GetDefaultBackgroundColor());
            ColorUtility.TryParseHtmlString("#ff756d", out var redred);
            //redred.a = 0.3f;
            ColorUtility.TryParseHtmlString("#ffcbd0", out var blue);
            blue.a = 0.3f;
            var white = Color.white;
            white.a = 0f;
            var grayTex = MakeTex(1, 1, white);
            var blueTex = MakeTex(1, 1, blue);
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
                //UnityEngine.UI.VerticalLayoutGroup
                Debug.LogWarning($"{editorWindow.GetType()}, {editorWindow.name}");
                var b = editorWindow.rootVisualElement;

                var a = Color.red;
                a.a = 0.2f;

                //UnityEngine.Cursor.SetCursor()
                //b.hierarchy.parent
                //var styleBackground = new UnityEngine.UIElements.StyleBackground(power.Texture);
                //b.parent[0].BringToFront();
                //var ower = b.parent[0].style;
                //ower.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
                //ower.unityBackgroundImageTintColor = new StyleColor(StyleKeyword.Null);
                //ower.backgroundColor = new StyleColor(StyleKeyword.Null);
                //ower.backgroundImage = styleBackground;
                //var c = b.parent[1].style;
                //c.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
                //c.unityBackgroundImageTintColor = new StyleColor(StyleKeyword.Null);
                //c.backgroundColor = new StyleColor(StyleKeyword.Null);
                //c.backgroundImage = styleBackground;
                var d = b.parent[0] as IMGUIContainer;
                var current = d.onGUIHandler;

                d.onGUIHandler = () =>
                {
                    if (editorWindow.titleContent.text == "Inspector")
                    {
                        UnityEngine.Debug.Log("!!");
                    }
                    var prop = typeof(GUISkin).GetField("current", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    //prop.SetValue(null, skin);

                    //GUI.skin = skin;
                    GUI.DrawTexture(d.contentRect, m_sprites[0], ScaleMode.ScaleAndCrop);
                    //if (!did)
                    //{
                    //    var gUISkin = ScriptableObject.CreateInstance<GUISkin>();
                    //    EditorUtility.CopySerialized(curr, gUISkin);

                    //    UnityEditor.ProjectWindowUtil.CreateAsset(gUISkin, "New GUISkin.guiskin");
                    //    //AssetDatabase.CreateAsset(gUISkin, "New GUISkin.guiskin");
                    //    //AssetDatabase.SaveAssets();
                    //    did = true;
                    //}
                    //UnityEditor.ProjectWindowUtil.CreateAsset(gUISkin, "New GUISkin.guiskin");
                    //curr.scrollView.normal.background = tex;
                    //curr.label.normal.background = tex;
                    //curr.box.normal.background = tex;
                    //curr.button.normal.background = tex;
                    //curr.textField.normal.background = tex;
                    //curr.textArea.normal.background = tex;
                    //curr.window.normal.background = tex;
                    //foreach (var ccc in curr.customStyles)
                    //{
                    //    ccc.normal.background = null;
                    //    ccc.onNormal.background = null;
                    //    ccc.normal.scaledBackgrounds = System.Array.Empty<Texture2D>();
                    //    ccc.onNormal.scaledBackgrounds = System.Array.Empty<Texture2D>();
                    //}
                    ////curr.label.normal.background = redTex;
                    //foreach (var power in curr.customStyles)
                    //{
                    //    power.normal.textColor = redred;
                    //    power.active.textColor = redred;
                    //    power.focused.textColor = redred;
                    //}
                    ////UnityEngine.Debug.LogWarning($"{curr.textField.normal.scaledBackgrounds.First().width}, {curr.textField.normal.scaledBackgrounds.First().height}");
                    ////curr.toggle.normal.scaledBackgrounds= System.Array.Empty<Texture2D>();
                    //var tex = curr.toggle.normal.background;

                    ////curr.toggle.normal.textColor = redred;
                    //curr.textField.normal.background = grayTex;
                    //curr.textField.normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("ObjectField")).normal.scaledBackgrounds = System.Array.Empty<Texture2D>();
                    //curr.customStyles.First(x => x.name.Contains("ObjectField")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("ObjectField")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("ObjectFieldButton")).normal.scaledBackgrounds = System.Array.Empty<Texture2D>();
                    //curr.customStyles.First(x => x.name.Contains("ObjectFieldButton")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("ObjectFieldButton")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("ControlLabel")).normal.scaledBackgrounds = System.Array.Empty<Texture2D>();
                    //curr.customStyles.First(x => x.name.Contains("ControlLabel")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("ControlLabel")).normal.textColor = redred;
                    ////curr.customStyles.First(x => x.name.Contains("AssetLabel")).normal.background = redTex;
                    ////curr.customStyles.First(x => x.name.Contains("IconButton")).normal.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("IN BigTitle")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("IN BigTitle")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("ControlLabel")).normal.background = null;
                    //curr.customStyles.First(x => x.name.Contains("Toolbar")).normal.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("dragtab first")).normal.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("dragtab")).normal.background = redTex;
                    ////curr.customStyles.First(x => x.name.Contains("ToolbarSeachTextFieldPopup")).normal.background = redTex;
                    ////curr.customStyles.First(x => x.name.Contains("ToolbarSeachCancelButtonEmpty")).normal.background = redTex;
                    //var dockHeader = curr.customStyles.First(x => x.name.Contains("dockHeader"));
                    ////curr.customStyles.First(x => x.name.Contains("MiniPopup")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("dockHeader")).normal.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("dockarea")).normal.background = null;
                    //curr.customStyles.First(x => x.name.Contains("SceneTopBarBg")).normal.background = blueTex;
                    //curr.customStyles.First(x => x.name.Contains("SceneTopBarBg")).active.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("ObjectField")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("ObjectFieldButton")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("RL Element")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("TV Line")).normal.background = null;
                    //curr.customStyles.First(x => x.name.Contains("TV Line")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("TV LineBold")).normal.background = null;
                    //curr.customStyles.First(x => x.name.Contains("TV LineBold")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("IN Foldout")).normal.background = null;
                    //curr.customStyles.First(x => x.name.Contains("IN Foldout")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("IN Title")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("IN Title")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("IN TitleText")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("Titlebar Foldout")).normal.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("Titlebar Foldout")).active.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("Titlebar Foldout")).focused.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("IN TitleText")).active.textColor = redred;
                    //curr.customStyles.First(x => x.name.Contains("IN TitleText")).focused.textColor = redred;
                    GUIStyle scrollViewAlt = "ScrollViewAlt";
                    var curScrollViewAlt = scrollViewAlt.normal.background;
                    scrollViewAlt.normal.background = grayTex;

                    GUIStyle dock = "dockarea";
                    var curDockArea = dock.normal.background;
                    dock.normal.background = grayTex;

                    GUIStyle tab = "TabWindowBackground";
                    var curTabWindowBackground = tab.normal.background;
                    tab.normal.background = grayTex;

                    //GUIStyle whiteBackground = "WhiteBackground";
                    //whiteBackground.normal.textColor = redred;
                    //whiteBackground.onNormal.textColor = redred;
                    //whiteBackground.onActive.textColor = redred;
                    //whiteBackground.onFocused.textColor = redred;
                    //whiteBackground.active.textColor = redred;
                    //whiteBackground.focused.textColor = redred;

                    //GUIStyle dockArea = "dockarea";
                    //dockArea.normal.background = null;
                    ////whiteBackground.onNormal.textColor = redred;
                    ////whiteBackground.onActive.textColor = redred;
                    ////whiteBackground.onFocused.textColor = redred;
                    ////whiteBackground.active.textColor = redred;
                    ////whiteBackground.focused.textColor = redred;
                    //var plplq = GetStyle("TabWindowBackground");
                    //curr.customStyles.First(x => x.name.Contains("ScrollViewAlt")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("ScrollViewAlt")).active.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("FoldoutHeader")).normal.background = grayTex;
                    //curr.customStyles.First(x => x.name.Contains("TabWindowBackground")).normal.background = null;
                    //curr.customStyles.First(x => x.name.Contains("CN EntryBackOdd")).normal.background = greenTex;
                    //curr.customStyles.First(x => x.name.Contains("CN EntryBackEven")).normal.background = redTex;
                    //curr.customStyles.First(x => x.name.Contains("CN EntryWarn")).normal.textColor = redred;
                    //curr.textField.active.textColor = redred;
                    //curr.settings.selectionColor = red;
                    //curr.box.focused.textColor = redred;
                    //curr.box.active.textColor = redred;
                    //curr.scrollView.focused.textColor = redred;
                    //curr.scrollView.active.textColor = redred;
                    //curr.textField.focused.textColor = redred;
                    //curr.textArea.active.textColor = redred;
                    //curr.textArea.focused.textColor = redred;
                    //curr.label.normal.textColor = redred;
                    ////curr.label.normal.background = redTex;
                    //curr.label.active.textColor = redred;
                    //curr.label.focused.textColor = redred;
                    ////curr.label.active.background = redTex;
                    //curr.button.normal.textColor = redred;
                    //curr.button.normal.background = redTex;
                    //curr.button.active.textColor = redred;
                    //curr.button.focused.textColor = redred;
                    //curr.button.active.background = redTex;

                    ////UnityEngine.Debug.Log(asdf.name);
                    ////var ddd = curr.customStyles.First(x => x.name == "GUIStyle \\'TabWindowBackground\\'");
                    ////curr.customStyles = new 
                    ////curr.scrollView.
                    ////curr. = 50;

                    //var asm = typeof(UnityEditor.ActiveEditorTracker).Assembly;
                    //var hohoho = asm.GetTypes().First(x => x.Name.Contains("DockArea"));
                    //var stylesTypeB = hohoho.GetNestedTypes(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    //var stylesType = stylesTypeB.First();
                    ////var dockBarStyle = stylesType.GetField("dockTitleBarStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    ////var dockBarGUIStyle = dockBarStyle.GetValue(null) as GUIStyle;
                    ////dockBarGUIStyle.normal.background = redTex;

                    //var backgroundStyle = stylesType.GetField("background", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    //var backgroundGUIStyle = backgroundStyle.GetValue(null) as GUIStyle;
                    //backgroundGUIStyle.normal.background = null;

                    //var tabLabelStyle = stylesType.GetField("tabLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    //var tabLabelGUIStyle = tabLabelStyle.GetValue(null) as GUIStyle;
                    ////tabLabelGUIStyle.normal.background = blueTex;
                    //tabLabelGUIStyle.normal.textColor = redred;

                    //var dragTabStyle = stylesType.GetField("dragTab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    //var dragTabGUIStyle = dragTabStyle.GetValue(null) as GUIStyle;
                    ////dragTabGUIStyle.normal.background = blueTex;
                    //dragTabGUIStyle.normal.textColor = redred;

                    //var dragTabFirstStyle = stylesType.GetField("dragTabFirst", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    //var dragTabFirstGUIStyle = dragTabFirstStyle.GetValue(null) as GUIStyle;
                    ////dragTabFirstGUIStyle.normal.background = blueTex;
                    //dragTabFirstGUIStyle.normal.textColor = redred;
                    ////prop.SetValue(curr);
                    ////GUISkin.current.
                    //GUIStyle a = "TV Selection";
                    //GUIStyle cddcc = "TabWindowBackground";
                    //cddcc.normal.background = null;
                    //a.normal.textColor = redred;
                    //a.active.textColor = redred;
                    //a.focused.textColor = redred;
                    //GUIStyle abcdef = "dockareaOverlay";
                    //abcdef.normal.background = null;
                    //UnityEditor.DockArea
                    //GUI.color = Color.blue;
                    current();
                    //GUI.color = Color.blue;

                    scrollViewAlt.normal.background = defaultColor;
                    dock.normal.background = defaultColor;
                    tab.normal.background = defaultColor;
                };
                //d.ClearClassList();

                //d.styleSheets.Clear();
                //c.backgroundColor = new UnityEngine.UIElements.StyleColor(Color.red);
                ////c.backgroundImage = styleBackground;

                ////b.style.backgroundColor = white;

                //c.unityBackgroundImageTintColor = white;
                //c.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
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
            var watch = new Stopwatch();
            watch.Start();
            Resources.FindObjectsOfTypeAll<EditorWindow>();

            watch.Stop();
            //UnityEngine.Debug.LogWarning(watch.ElapsedMilliseconds);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                c();
            }
        }
    }
}
