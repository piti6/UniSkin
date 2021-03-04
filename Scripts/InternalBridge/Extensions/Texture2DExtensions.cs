using UnityEngine;

namespace UniSkin
{
    internal static class Texture2DExtensions
    {
        public static Texture2D ToDecompressedTexture(this Texture2D source)
        {
            var renderTexture = RenderTexture.GetTemporary(source.width, source.height);

            Graphics.Blit(source, renderTexture);

            var readableTexture = new Texture2D(source.width, source.height);
            readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            readableTexture.Apply();

            RenderTexture.ReleaseTemporary(renderTexture);

            return readableTexture;
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
