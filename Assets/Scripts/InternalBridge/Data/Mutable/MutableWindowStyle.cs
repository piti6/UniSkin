using System.Collections.Generic;
using System.Linq;

namespace UniSkin
{
    internal class MutableWindowStyle
    {
        public string Name { get; set; }
        public string CustomBackgroundId { get; set; }
        public Dictionary<string, MutableElementStyle> ElementStyles { get; set; }

        public MutableWindowStyle(WindowStyle windowStyle) : this(windowStyle.Name, windowStyle.CustomBackgroundTextureId, windowStyle.ElementStyles.Values.ToArray())
        { }

        public MutableWindowStyle(string name, string customBackgroundId, ElementStyle[] elementStyles)
        {
            Name = name;
            CustomBackgroundId = customBackgroundId;
            ElementStyles = elementStyles.ToDictionary(x => x.Name, x => new MutableElementStyle(x));
        }

        public WindowStyle ToImmutable()
        {
            return new WindowStyle(Name, CustomBackgroundId, ElementStyles.Values.Select(x => x.ToImmutable()).ToArray());
        }
    }
}
