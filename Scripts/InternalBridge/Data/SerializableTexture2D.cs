using System;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    internal class SerializableTexture2D
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
                if (IsValid && m_texture == default)
                {
                    m_texture = new Texture2D(m_width, m_height);
                    m_texture.LoadImage(m_byte);
                }

                return m_texture;
            }
        }

        public SerializableTexture2D(string id, Texture2D texture)
        {
            m_id = id;
            m_width = texture.width;
            m_height = texture.height;
            m_texture = texture;
            m_byte = IsValid ? ImageConversion.EncodeToPNG(texture.ToDecompressedTexture()) : Array.Empty<byte>();
        }
    }
}
