using System;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class SerializableTexture2D : ISerializationCallbackReceiver
    {
        [SerializeField]
        private int _id = default;
        [SerializeField]
        private int _width = default;
        [SerializeField]
        private int _height = default;
        [SerializeField]
        private byte[] _byte = default;

        public bool IsValid => _id != default;

        private Texture2D _texture = default;
        public Texture2D Texture
        {
            get
            {
                if (_texture == default)
                {
                    _texture = new Texture2D(_width, _height);
                    _texture.LoadImage(_byte);
                }

                return _texture;
            }
        }

        public SerializableTexture2D() { }
        public SerializableTexture2D(int id, Texture2D texture)
        {
            _id = id;
            _texture = texture;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (IsValid)
            {
                _byte = ImageConversion.EncodeToPNG(_texture.MakeReadable());
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
