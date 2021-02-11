using System.Collections.Generic;
using System.Linq;

namespace UniSkin
{
    public class MutableSkin
    {
        public string Id { get; }
        public string Name { get; set; }
        public Dictionary<string, SerializableTexture2D> Textures { get; set; }
        public Dictionary<string, MutableWindowStyle> WindowStyles { get; set; }

        public MutableSkin(Skin skin) : this(skin.Name, skin.Textures.Values.ToArray(), skin.WindowStyles.Values.ToArray())
        { }

        public MutableSkin(string name, SerializableTexture2D[] textures, WindowStyle[] windowStyles)
        {
            Name = name;
            Textures = textures.ToDictionary(x => x.Id);
            WindowStyles = windowStyles.ToDictionary(x => x.Name, x => new MutableWindowStyle(x));
        }

        public override bool Equals(object obj)
        {
            return obj is MutableSkin skin &&
                   Name == skin.Name &&
                   Textures.SequenceEqual(skin.Textures) &&
                   WindowStyles.SequenceEqual(skin.WindowStyles);
        }

        public override int GetHashCode()
        {
            int hashCode = -624386006;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, SerializableTexture2D>>.Default.GetHashCode(Textures);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, MutableWindowStyle>>.Default.GetHashCode(WindowStyles);
            return hashCode;
        }

        public static bool operator ==(MutableSkin left, MutableSkin right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MutableSkin left, MutableSkin right)
        {
            return !(left == right);
        }

        public Skin ToImmutable()
        {
            return new Skin(Id, Name, Textures.Values.ToArray(), WindowStyles.Values.Select(x => x.ToImmutable()).ToArray());
        }
    }
}
