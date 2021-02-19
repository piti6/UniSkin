# UniSkin

[![CodeFactor](https://www.codefactor.io/repository/github/piti6/uniskin/badge?s=f9067ada8527e600dfed06fa997c7011d0c95735)](https://www.codefactor.io/repository/github/piti6/uniskin)
[![Unity](https://img.shields.io/badge/Unity-2019.4+-brightgreen)](https://unity3d.com/kr/unity/qa/lts-releases?version=2019.4)

![Screenshot](https://user-images.githubusercontent.com/19143280/108381460-2bed0780-724b-11eb-9f0c-90ce8226edeb.png)

UniSkin과 함께 당신만의 유니티를 만들어보세요  :)

#### UniSkin은 현재 프리뷰 버전입니다.
#### 프리뷰 버전은 각종 버그를 포함하고 있을 수 있으며, 안정된 동작을 보증하지 않습니다.

## 기능

- EditorWindow마다 완전히 커스터마이즈 가능한 GuiStyle적용가능
  - 단색 배경
  - 텍스쳐 배경
  - 반투명 배경
  - 폰트 사이즈
  - 폰트 컬러
  - 폰트 스타일

- EditorWindow마다 추가로 두장의 텍스쳐 배경을 제공합니다.
- 스킨 파일은 단일 파일로써, 편리하게 다른 사용자와 스킨을 공유할 수 있습니다.

## 설치

### 요구사항
#### 지원하는 유니티 버전
- Unity 2019.4+
- 테스트 완료
  - Unity 2019.4.17f1
  - Unity 2020.2.1f1
  - Unity 2021.1.0b6
- 추후 대응 예정
  - Unity 2018.4+
- 대응 예정 없음
  - ~ Unity 2017

### UPM을 이용한 설치 (Unity Package Manager)
- 자동 설치
  - 유니티 상부 메뉴에서, Window -> Package Manager를 선택 해 주세요.
  - Package Manager의 왼쪽위에 위치한 +버튼을 눌러,「Add package from git URL」메뉴를 선택 해 주세요.
  - 「https://github.com/piti6/UniSkin.git」을 입력하고 Add버튼을 눌러주세요.
- 수동 설치
  - manifest.json파일을 편집기로 열고, dependencies이하에
`"space.mkim.uniskin": "https://github.com/piti6/UniSkin.git"` 를 기입해 주세요.

## Quickstart

- 유니티 상부 메뉴에서 Window -> UniSkin를 선택하면 스킨 편집 윈도우를 열 수 있습니다.
- 「Load from file」을 선택 해 주세요.
- 샘플로 제공되는「Sample.skn」파일을 선택하면 샘플 스킨이 적용됩니다.

## 시작하기

### 인스턴스 관리 정책
- 기본적으로, UniSkin은 이하와 같이 유니티의 레이아웃 시스템의 정책을 따라갑니다.
  - UniSkin은 컴퓨터 내에서 모든 유니티 버전과 프로젝트를 통틀어 하나의 스킨이 적용됩니다.
  - 현재 스킨을 파일을 통해 보존하거나, 파일에서 스킨을 불러올 수 있습니다.

### 스킨 편집 윈도우

### 톱 페이지
![image](https://user-images.githubusercontent.com/19143280/108517172-6d90b780-730a-11eb-9ac5-ebf33565a76b.png)

1. 스킨이름을 변경합니다.
2. 현재 편집한 변경사항을 스킨 인스턴스에 적용합니다.
3. 현재 스킨 인스턴스와 변경사항을 파일로 보존합니다.
4. 파일에서 스킨을 로드해서 현재 스킨인스턴스 위에 덮어씁니다.
5. 현재 편집한 변경사항을 파기합니다.
6. 현재 스킨 인스턴스를 파기하고, 기본 유니티 상태로 되돌아갑니다.
7. 편집하고 싶은 에디터 윈도우를 선택합니다. 팝업 리스트는 현재 활성화 되어있는 모든 에디터 윈도우를 표시합니다. (만약에 특정 윈도우를 편집하고 싶으시다면, 그 윈도우를 메뉴에서 선택해서 표시해야 할 필요가 있습니다.)

### 상세뷰
![image](https://user-images.githubusercontent.com/19143280/108518593-14c21e80-730c-11eb-8e78-b40079bd7608.png)

*시작전에*
- 편집중인 윈도우의 UI가 갱신되면, 스킨 편집 윈도우의 상세뷰가 새로고침 됩니다.
- 따라서, 현재 상세뷰에 표시되어 있는 항목을 모두 편집했다고 해서 해당 윈도우에 있는 모든 스타일을 편집한 것은 아니라는 점을 명심 해 주세요. 아직 해당 윈도우에 표시되지 않은 항목이 있다면, 해당 항목은 편집되지 않았을 가능성이 있습니다. (예를 들면, 유니티의 인스펙터는 정말 많은 종류의 정보를 담고 있기 때문에 만약 인스펙터의 모든 스타일을 편집하려 한다면 많은 노력이 필요하게 됩니다) 

1. 커스텀 텍스쳐 배경
  - GUI.DrawTexture(ScaleMode.ScaleAndCrop); 를 통해 그려지는 커스텀 텍스쳐 배경을 설정할 수 있습니다.
  - 기본적으로는 이 기능을 사용하여 배경을 설정하게 될 텐데요, GuiStyle의 backgroundTexture는 uses ScaleMode.StretchToFill모드를 사용하기 때문입니다. (윈도우를 늘리거나 하면 찌그러져요)
  - 보통 커스텀 텍스쳐 배경을 설정하시더라도 바로 보이지 않는데요, 왜냐하면 커스텀 텍스쳐 배경보다 위에 많은 종류의 UI가 그려지기 때문입니다. 커스텀 텍스쳐 배경을 보이게 하시려면, 앞에서 가리고 있는 항목들을 투명하게 해줄 필요가 있습니다.「3. 스타일 편집」을 참고해 주세요.
2. 인스트럭션 선택 윈도우
  - 현재 선택중인 윈도우의 특정 스타일을 선택합니다.
  - 동일한 스타일을 사용중인 항목은 함께 편집됩니다.
  - 인스트럭션을 선택할 경우, 해당하는 UI가 하이라이트 되어 자신이 어떤 항목을 편집하고 있는지 확인할 수 있습니다.
3. 스타일 편집
  - 선택한 스타일을 편집합니다.
  - FontSize: 폰트 사이즈를 변경합니다. 선택한 스타일이 텍스트에 사용되지 않는다면, 당연한 얘기이지만 아무일도 일어나지 않습니다.
  - FontStyle: 볼드, 이탤릭 같은 폰트의 속성을 변경합니다.
  - Normal/Hover/Active/Focused/OnNormal/OnHover/OnActive/OnFocused
    - https://docs.unity3d.com/ScriptReference/GUIStyle.html GUIStyle의 각 항목과 완전히 일치합니다. 「On」이 붙는 항목들 (OnNormal, OnHover..) 은 On/Off 상태가 있는, 토글이나 체크박스에서 사용됩니다.
  - 위에 상태중 하나를 선택했을때
    - Texture/Color
      - Texture모드에서는, 선택한 스타일의 배경으로 텍스쳐를 사용할 수 있습니다. ScaleMode.StretchToFill을 사용해 그린다는 점에 주의하세요. (찌그러집니다)
      - Color모드에서는, 단색 배경의 설정이 가능합니다. 알파값도 적용됩니다. **따라서, 커스텀 텍스쳐 배경의 방해가 되는 항목을 투명하게 함으로써 커스텀 텍스쳐 배경을 표시하게 할 수 있습니다.**
    - TextColor: 글씨의 색을 바꿉니다. 알파값도 적용됩니다.

### 기여하기
- 이슈/PR은 언제나 환영합니다 :)
- 저는 디자이너가 아니기 때문에, 샘플 스킨을 만드는데 애를 먹고있습니다. 만약 간단한 샘플을 제작해 주실 수 있는 분이 계신다면, 감사히 도움을 받고 싶습니다. (저작권에 문제가 되지 않는 한에서)

