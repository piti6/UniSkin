# UniSkin

[![CodeFactor](https://www.codefactor.io/repository/github/piti6/uniskin/badge?s=f9067ada8527e600dfed06fa997c7011d0c95735)](https://www.codefactor.io/repository/github/piti6/uniskin)
[![Unity](https://img.shields.io/badge/Unity-2019.4+-brightgreen)](https://unity3d.com/kr/unity/qa/lts-releases?version=2019.4)

![Screenshot](https://user-images.githubusercontent.com/19143280/108381460-2bed0780-724b-11eb-9f0c-90ce8226edeb.png)

Make your custom editor skin with UniSkin :)

#### Currently UniSkin is in preview.
#### Preview version may contain: Major bugs, and does not guarantee stable behavior.

## Features

- Apply fully customized GUISkin per EditorWindow GuiStyle
  - Colored background
  - Textured background
  - Transparent background
  - Font size
  - Font color
  - Font style

- Additional two custom texture background per EditorWindow
- Uses single file to preserve skin (.skn), so you can handle skin file easily

## Installation

### Requirements
#### Supported Unity version
- Unity 2019.4+
- Tested
  - Unity 2019.4.17f1
  - Unity 2020.2.1f1
  - Unity 2021.1.0b6
- On the roadmap
  - Unity 2018.4+
- Not supported
  - ~ Unity 2017

### Install via UPM
- Automatic
  - Open Package Manager Window at Unity menu Window -> Package Manager
  - Select「Add package from git URL」menu at the left top button
  - Type 「https://github.com/piti6/UniSkin.git」 and hit Add button
- Manual
  - Include next line inside of manifest.json dependencies.
`"space.mkim.uniskin": "https://github.com/piti6/UniSkin.git"`

## Quickstart

- Select Unity menu Window -> UniSkin opens Skin editor window.
- Select 「Load from file」
- Select 「Sample.skn」file loads sample skin preset.

## Getting started

### Instance policy
- Basically, UniSkin follows Unity layout system policy, as follows:
  - UniSkin shares one skin per computer, across overall Unity versions and projects.
  - You can save current skin as file or you can load skin from file.

### Skin Editor Window

### Top page
![image](https://user-images.githubusercontent.com/19143280/108517172-6d90b780-730a-11eb-9ac5-ebf33565a76b.png)

1. Changes skin name.
2. Save current changes to current skin instance.
3. Save current skin instance and changes to file.
4. Load skin from file and overwrites to current skin instance.
5. Revert current edit changes.
6. Remove current skin and restore default Unity skin.
7. Select EditorWindow to inspect. Popup list shows current activated windows. (If you want to customize specific EditorWindow, you should open that window.)

### Inspect view
![image](https://user-images.githubusercontent.com/19143280/108518593-14c21e80-730c-11eb-8e78-b40079bd7608.png)

*Before to start*
- Changes in selected inspected window will referesh instructions list.
- That is, edited all of instructions on current instructions list does not mean that every style was completely edited. there will be more that if you didn't check all possible elements on that EditorWindow (e.g Inspector window has plenty of GuiStyle, which is pain) 

1. Custom background texture
  - You can add custom background textures rendered by GUI.DrawTexture(ScaleMode.ScaleAndCrop);
  - Basically you'd prefer to use these to show backgrounds, as GuiStyle backgroundTexture uses ScaleMode.StretchToFill.
  - Usually there's plenty of non-transparent elements on EditorWindow that hides custom backgrounds. To show custom backgrounds, you should make them completely transparent status. check 「3. Edit style」section.
2. Instruction select window
  - Select specific style to edit current Inspected window style.
  - Same style will be edited together.
  - If you select instruction, elements that uses same GuiStyle on EditorWindow will be highlighted so that you can recognizes which one you are working on.
3. Edit style
  - Edit current selected elements style.
  - FontSize: changes font size. note that if selected style does not used for text display, of course it does nothing at all.
  - FontStyle: changes font style. (attribute, such as bold/italic)
  - Normal/Hover/Active/Focused/OnNormal/OnHover/OnActive/OnFocused
    - Checkout https://docs.unity3d.com/ScriptReference/GUIStyle.html, it acts exactly same as it is. 「On」 series (OnNormal, OnHover, etc..) usually used in controls that has On/Off statement, such as Toggle/Checkbox.
  - Inside Foldout of one of above style
    - Texture/Color
      - On Texture mode, you can use Texture as selected style background. note that this renders texture as ScaleMode.StretchToFill.
      - On Color mode, you can use Color as selected style background. Alpha value also affects to display, **so you can use full-transparent color to hide current style background. which needs to show custom background textures.**
    - TextColor: changes text color. Alpha value also affects to display.

### Contribution
- Issues/Pull requests are always welcome :)
- I'm having trouble to decorate sample skin as I'm not a designer, so I appreciate your help to make a custom sample skin. (Which does not violates copyright)

