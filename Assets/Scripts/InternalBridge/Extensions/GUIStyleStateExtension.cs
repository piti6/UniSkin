using UnityEngine;

namespace UniSkin
{
    public static class GUIStyleStateExtension
    {
        public static StyleState ToStyleState(this GUIStyleState guiStyleState, StyleStateType stateType)
        {
            return new StyleState(stateType, BackgroundType.Texture, guiStyleState.background.ToTextureId(), Color.white, guiStyleState.textColor);
        }
    }
}
