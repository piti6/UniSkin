using System;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class SerializableTexture2D : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string m_id = default;
        [SerializeField]
        private int m_width = default;
        [SerializeField]
        private int m_height = default;
        [SerializeField]
        private byte[] m_byte = default;

        public bool IsValid => m_id != default;
        public string Id => m_id;

        private Texture2D m_texture = default;
        public Texture2D Texture
        {
            get
            {
                if (m_texture == default)
                {
                    m_texture = new Texture2D(m_width, m_height);
                    m_texture.LoadImage(m_byte);
                }

                return m_texture;
            }
        }

        public SerializableTexture2D() { }
        public SerializableTexture2D(string id, Texture2D texture)
        {
            m_id = id;
            m_texture = texture;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (IsValid)
            {
                m_byte = ImageConversion.EncodeToPNG(m_texture.MakeReadable());
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
