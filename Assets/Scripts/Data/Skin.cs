using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class Skin
    {
        public IReadOnlyList<SerializableTexture2D> Textures => _textures;
        public IReadOnlyDictionary<string, WindowStyle> WindowStyles => _windowStyleDictionary ?? (_windowStyleDictionary = _windowStyles.ToDictionary(x => x.Name));

        private Dictionary<string, WindowStyle> _windowStyleDictionary;

        [SerializeField]
        private SerializableTexture2D[] _textures;
        [SerializeField]
        private WindowStyle[] _windowStyles;

        public Skin() { }
        public Skin(SerializableTexture2D[] textures, WindowStyle[] windowStyles)
        {
            _textures = textures;
            _windowStyles = windowStyles;
        }
    }
}
