using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    internal class Skin
    {
        private static readonly Lazy<Skin> m_default = new Lazy<Skin>(() => new Skin(string.Empty, "Default", Array.Empty<SerializableTexture2D>(), Array.Empty<WindowStyle>()));
        public static Skin Default => m_default.Value;

        public string Id => m_id;
        public string Name => m_name;
        public IReadOnlyDictionary<string, SerializableTexture2D> Textures => m_textureDictionary ?? (m_textureDictionary = m_textures.ToDictionary(x => x.Id));
        public IReadOnlyDictionary<string, WindowStyle> WindowStyles => m_windowStyleDictionary ?? (m_windowStyleDictionary = m_windowStyles.ToDictionary(x => x.Name));

        private IReadOnlyDictionary<string, SerializableTexture2D> m_textureDictionary;
        private IReadOnlyDictionary<string, WindowStyle> m_windowStyleDictionary;

        [SerializeField]
        private string m_id;
        [SerializeField]
        private string m_name;
        [SerializeField]
        private SerializableTexture2D[] m_textures;
        [SerializeField]
        private WindowStyle[] m_windowStyles;

        public Skin(string id, string name, SerializableTexture2D[] textures, WindowStyle[] windowStyles)
        {
            m_id = id;
            m_name = name;
            m_textures = textures;
            m_windowStyles = windowStyles;
        }

        public override bool Equals(object obj)
        {
            return obj is Skin skin &&
                m_id == skin.m_id &&
                m_name == skin.m_name &&
                m_textures.SequenceEqual(skin.m_textures) &&
                m_windowStyles.SequenceEqual(skin.m_windowStyles);
        }

        public override int GetHashCode()
        {
            int hashCode = -624386006;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_name);
            hashCode = hashCode * -1521134295 + EqualityComparer<SerializableTexture2D[]>.Default.GetHashCode(m_textures);
            hashCode = hashCode * -1521134295 + EqualityComparer<WindowStyle[]>.Default.GetHashCode(m_windowStyles);
            return hashCode;
        }

        public static bool operator ==(Skin left, Skin right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Skin left, Skin right)
        {
            return !(left == right);
        }
    }
}
