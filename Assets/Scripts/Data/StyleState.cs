using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class StyleState
    {
        public StyleStateType SkinElementState => _skinElementState;
        public int BackgroundTextureId => _backgroundTextureId;
        public Color TextColor => _textColor;
        public IReadOnlyList<int> ScaledBackgroundTextureIds => _scaledBackgroundTextureIds;

        [SerializeField]
        private StyleStateType _skinElementState;
        [SerializeField]
        private int _backgroundTextureId;
        [SerializeField]
        private Color _textColor;
        [SerializeField]
        private int[] _scaledBackgroundTextureIds;
    }
}
