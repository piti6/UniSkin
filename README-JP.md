# UniSkin

[![CodeFactor](https://www.codefactor.io/repository/github/piti6/uniskin/badge?s=f9067ada8527e600dfed06fa997c7011d0c95735)](https://www.codefactor.io/repository/github/piti6/uniskin)
[![Unity](https://img.shields.io/badge/Unity-2019.4+-brightgreen)](https://unity3d.com/kr/unity/qa/lts-releases?version=2019.4)

![Screenshot](https://user-images.githubusercontent.com/19143280/108381460-2bed0780-724b-11eb-9f0c-90ce8226edeb.png)

自分だけのカスタムスキンを作りましょう :)

#### 現在、UniSkinはプレビュー版です。
#### プレビュー版は様々なバグを含んでいる可能性があり、安定した動作は保証しません。

## 特徴

- ウィンドウごとに完全なカスタマイズ可能
  - 色背景
  - 画像背景
  - (半)透明背景
  - フォントサイズ
  - フォントカラー
  - フォントスタイル

- ウィンドウごとに追加で二枚の画像背景表示可能
- 扱いやすい単一ファイルフォーマット(.skn)

## インストール

### 要求事項
#### サポートするユニティバージョン
- Unity 2019.4+
- テスト済み
  - Unity 2019.4.17f1
  - Unity 2020.2.1f1
  - Unity 2021.1.0b6
- 対応予定
  - Unity 2018.4+
- 対応予定なし
  - ~ Unity 2017

### UPM (Unity Package Manager)を用いたインストール
- 自動インストール
  - Unity上部メニューの Window -> Package Managerを選択
  - 左上の「+」ボタンを押し、「Add package from git URL」項目を選択
  - 「https://github.com/piti6/UniSkin.git」を入力し、Addボタン選択
- 手動インストール
  - 次をmanifest.jsonのdependencies以下に記入
`"space.mkim.uniskin": "https://github.com/piti6/UniSkin.git"`

## Quickstart

- Unity上部メニューの Window -> UniSkinを選択するとスキン編集ウィンドウが開きます。
- 「Load from file」を選択するとスキンファイルを選択出来ます。
- 「Sample.skn」ファイルを選択すると、サンプルのスキンが適用されます。

## Getting started

### スキンインスタンスの扱い
- 基本的にはUniSkinは以下のようにユニティのレイアウトシステムの方針と同じ方針を取っています。
  - UniSkinのスキンはパソコン全体を跨いで共有されます。全てのユニティバージョンとプロジェクトを跨いで一つです。
  - 現在のスキンをファイルとして書き込むか、ファイルから読み込むことが可能です。

### スキン編集ウィンドウ

### Top page
![image](https://user-images.githubusercontent.com/19143280/108517172-6d90b780-730a-11eb-9ac5-ebf33565a76b.png)

1. スキンの命名を変えます。
2. 現状の変更をスキンインスタンスに保存します。
3. 現状のスキンインスタンスと変更をファイルとして保存します。
4. ファイルからスキンを読み込み、現在のスキンインスタンスに上書きします。
5. 現状の変更を破棄します。
6. 現状のスキンインスタンスを破棄し、通常のユニティ状態に戻します。
7. 編集したいEditorWindowを選択します。リストは現状表示されているEditorWindowを閲覧します。(特定のウィンドウを編集したいのであれば、そのウィンドウを開く必要があります)

### Inspect view
![image](https://user-images.githubusercontent.com/19143280/108518593-14c21e80-730c-11eb-8e78-b40079bd7608.png)

*始まる前に*
- 選択したウィンドウにかかった変更に応じて現状のinstructionリストが更新されます。
- ということは、現状表示されているinstructionを全部編集したとしてもあり得る全部のスタイルを編集したわけではなく、現状表示されていないだけでまだ修正をかけていないスタイルがある可能性があります。(例えばインスペクターウィンドウは場合によって表示する情報の量が非常に多く、完璧に全てのスタイルを編集したい場合は多くの作業が必要となります。)

1. カスタム画像背景
  - 二枚まで、GUI.DrawTexture(ScaleMode.ScaleAndCrop);を用いて描画される背景を設定出来ます。
  - 基本的に背景を表示する際にはこちらの設定を使うことをお勧めします。(後述するGuiStyleのbackgroundTextureはScaleMode.StretchToFillを使うからです。)
  - 普段はEditorWindowに多くのものが表示されるため、設定したカスタム画像背景を隠すことがあります。カスタム画像背景を表示するためには、3番項目で説明する方法で背景を隠すエレメントを完全に透明にする必要があります。
2. Instruction選択ウィンドウ
  - 特定のInstructionを選択し、右に表示されるスタイルを編集します。
  - 同ウィンドウ内の同一スタイルは一緒に編集されます。
  - Instructionを選択すると、当ウィンドウで該当する項目がハイライトされ、編集しようとする項目がどれなのかが認識できます。
3. スタイル編集
  - 現状選択した項目のスタイルを編集します。
  - FontSize: フォントサイズを編集します。
  - FontStyle: フォントスタイルを編集します。(bold/italicなど)
  - Normal/Hover/Active/Focused/OnNormal/OnHover/OnActive/OnFocused
    - https://docs.unity3d.com/ScriptReference/GUIStyle.html の説明と同様に動作します。「On」 がつくものに関しては(OnNormal, OnHover等々)、OnとOffの状態があるものに対して適用されます。(Toggle/Checkboxなど)
  - 上記スタイルを選択した場合
    - Texture/Color
      - Textureを選択した場合、Textureを選択したスタイルの背景として使うことが出来ます。ScaleMode.StretchToFillを用いて描画することにご注意ください。
      - Colorを選択した場合、背景として色を設定することが出来ます。アルファ値も効きます。**アルファを0にすると透明になるので、カスタム画像背景を使いたい場合は邪魔になる項目を透明にすることが必要となります。**
    - TextColor: テキストのカラーを変えます。アルファ値も効きます。

## Contribution
- Issue/PRはいつでも大歓迎です :)
- サンプルスキンを作るのに困っているため、サンプルスキンの制作を手伝って頂けると非常に助かります。(著作権に引っかかるものはNGとさせて頂きます。)

