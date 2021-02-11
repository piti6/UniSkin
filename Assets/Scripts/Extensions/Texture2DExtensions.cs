using UnityEngine;

namespace UniSkin
{
    public static class Texture2DExtensions
    {
        public static Texture2D MakeReadable(this Texture2D source)
        {
            if (source == null || source.isReadable)
            {
                return source;
            }
            else
            {
                var newTexture2D = new Texture2D(source.width, source.height, source.format, source.mipmapCount, source);
                Graphics.CopyTexture(source, newTexture2D);

                return newTexture2D;
            }
        }

        public static string ToTextureId(this Texture2D source)
        {
            if (source == null) return null;
            else return $"{source.name}{source.width}{source.height}";
        }

        public static SerializableTexture2D ToSerializableTexture2D(this Texture2D source)
        {
            return new SerializableTexture2D(source.ToTextureId(), source);
        }
    }
}
