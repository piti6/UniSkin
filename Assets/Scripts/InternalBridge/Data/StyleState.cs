using System;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class StyleState
    {
        public StyleStateType StateType => _stateType;
        public BackgroundType BackgroundType => _backgroundType;
        public string BackgroundTextureId => _backgroundTextureId;
        public Color BackgroundColor => _backgroundColor;
        public Color TextColor => _textColor;

        [SerializeField]
        private StyleStateType _stateType;
        [SerializeField]
        private BackgroundType _backgroundType;
        [SerializeField]
        private string _backgroundTextureId;
        [SerializeField]
        private Color _backgroundColor;
        [SerializeField]
        private Color _textColor;

        public StyleState(StyleStateType stateType, BackgroundType backgroundType, string backgroundTextureId, Color backgroundColor, Color textColor)
        {
            _stateType = stateType;
            _backgroundType = backgroundType;
            _backgroundTextureId = backgroundTextureId;
            _backgroundColor = backgroundColor;
            _textColor = textColor;
        }
    }
}
