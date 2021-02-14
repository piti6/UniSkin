using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    public class MutableStyleState
    {
        public StyleStateType StateType { get; }
        public string BackgroundTextureId { get; set; }
        public Color TextColor { get; set; }
        public List<string> ScaledBackgroundTextureIds { get; set; }

        public MutableStyleState(StyleState styleState) : this(styleState.StateType, styleState.BackgroundTextureId, styleState.TextColor, styleState.ScaledBackgroundTextureIds.ToArray())
        { }

        public MutableStyleState(StyleStateType stateType, string backgroundTextureId, Color textColor, string[] scaledBackgroundTextureIds)
        {
            StateType = stateType;
            BackgroundTextureId = backgroundTextureId;
            TextColor = textColor;
            ScaledBackgroundTextureIds = scaledBackgroundTextureIds.ToList();
        }

        public StyleState ToImmutable()
        {
            return new StyleState(StateType, BackgroundTextureId, TextColor, ScaledBackgroundTextureIds.ToArray());
        }
    }
}
