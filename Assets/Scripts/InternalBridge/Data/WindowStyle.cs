using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    internal class WindowStyle
    {
        public string Name => _name;
        public string CustomBackgroundTextureId => _customBackgroundTextureId;
        public string CustomBackgroundTextureId2 => _customBackgroundTextureId2;
        public IReadOnlyDictionary<string, ElementStyle> ElementStyles => _elementStyleDictionary ?? (_elementStyleDictionary = _elementStyles.ToDictionary(x => x.Name));

        private IReadOnlyDictionary<string, ElementStyle> _elementStyleDictionary;

        [SerializeField]
        private string _name;
        [SerializeField]
        private string _customBackgroundTextureId;
        [SerializeField]
        private string _customBackgroundTextureId2;
        [SerializeField]
        private ElementStyle[] _elementStyles;

        public WindowStyle(string name, string customBackgroundTextureId, string customBackgroundTextureId2, ElementStyle[] elementStyles)
        {
            _name = name;
            _customBackgroundTextureId = customBackgroundTextureId;
            _customBackgroundTextureId2 = customBackgroundTextureId2;
            _elementStyles = elementStyles;
        }
    }
}
