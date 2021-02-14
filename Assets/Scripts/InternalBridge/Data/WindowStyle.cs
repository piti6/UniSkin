using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class WindowStyle
    {
        public string Name => _name;
        public IReadOnlyDictionary<string, ElementStyle> ElementStyles => _elementStyleDictionary ?? (_elementStyleDictionary = _elementStyles.ToDictionary(x => x.Name));

        private IReadOnlyDictionary<string, ElementStyle> _elementStyleDictionary;

        [SerializeField]
        private string _name;
        [SerializeField]
        private ElementStyle[] _elementStyles;

        public WindowStyle() { }
        public WindowStyle(string name, ElementStyle[] elementStyles)
        {
            _name = name;
            _elementStyles = elementStyles;
        }
    }
}
