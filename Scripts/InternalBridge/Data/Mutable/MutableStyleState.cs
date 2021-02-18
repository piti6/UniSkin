using UnityEngine;

namespace UniSkin
{
    internal class MutableStyleState
    {
        public StyleStateType StateType { get; }
        public BackgroundType BackgroundType { get; set; }
        public string BackgroundTextureId { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }

        public MutableStyleState(StyleState styleState) : this(styleState.StateType, styleState.BackgroundType, styleState.BackgroundTextureId, styleState.BackgroundColor, styleState.TextColor)
        { }

        public MutableStyleState(StyleStateType stateType, BackgroundType backgroundType, string backgroundTextureId, Color backgroundColor, Color textColor)
        {
            StateType = stateType;
            BackgroundType = backgroundType;
            BackgroundTextureId = backgroundTextureId;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
        }

        public StyleState ToImmutable()
        {
            return new StyleState(StateType, BackgroundType, BackgroundTextureId, BackgroundColor, TextColor);
        }
    }
}
