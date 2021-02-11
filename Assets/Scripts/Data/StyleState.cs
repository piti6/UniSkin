using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class StyleState
    {
        public StyleStateType StateType => _stateType;
        public string BackgroundTextureId => _backgroundTextureId;
        public Color TextColor => _textColor;
        public IReadOnlyList<string> ScaledBackgroundTextureIds => _scaledBackgroundTextureIds;

        [SerializeField]
        private StyleStateType _stateType;
        [SerializeField]
        private string _backgroundTextureId;
        [SerializeField]
        private Color _textColor;
        [SerializeField]
        private string[] _scaledBackgroundTextureIds;

        public StyleState(StyleStateType stateType, string backgroundTextureId, Color textColor, string[] scaledBackgroundTextureIds)
        {
            _stateType = stateType;
            _backgroundTextureId = backgroundTextureId;
            _textColor = textColor;
            _scaledBackgroundTextureIds = scaledBackgroundTextureIds;
        }
    }
}
