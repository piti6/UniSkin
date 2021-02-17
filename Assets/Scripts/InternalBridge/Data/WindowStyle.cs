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
        public IReadOnlyDictionary<string, ElementStyle> ElementStyles => _elementStyleDictionary ?? (_elementStyleDictionary = _elementStyles.ToDictionary(x => x.Name));

        private IReadOnlyDictionary<string, ElementStyle> _elementStyleDictionary;

        [SerializeField]
        private string _name;
        [SerializeField]
        private string _customBackgroundTextureId;
        [SerializeField]
        private ElementStyle[] _elementStyles;

        public WindowStyle(string name, string customBackgroundTextureId, ElementStyle[] elementStyles)
        {
            _name = name;
            _customBackgroundTextureId = customBackgroundTextureId;
            _elementStyles = elementStyles;
        }
    }
}
