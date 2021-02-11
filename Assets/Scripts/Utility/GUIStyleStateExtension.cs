using System.Linq;
using UnityEngine;

namespace UniSkin
{
    public static class GUIStyleStateExtension
    {
        public static StyleState ToStyleState(this GUIStyleState guiStyleState, StyleStateType stateType)
        {
            return new StyleState(stateType, guiStyleState.background.ToTextureId(), guiStyleState.textColor, guiStyleState.scaledBackgrounds.Where(x => x != null).Select(x => x.ToTextureId()).ToArray());
        }
    }
}
