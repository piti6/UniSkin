using System.Collections.Generic;
using System.Linq;

namespace UniSkin
{
    internal class MutableWindowStyle
    {
        public string Name { get; set; }
        public string CustomBackgroundId { get; set; }
        public string CustomBackgroundId2 { get; set; }
        public Dictionary<string, MutableElementStyle> ElementStyles { get; set; }

        public MutableWindowStyle(WindowStyle windowStyle) : this(windowStyle.Name, windowStyle.CustomBackgroundTextureId, windowStyle.CustomBackgroundTextureId2, windowStyle.ElementStyles.Values.ToArray())
        { }

        public MutableWindowStyle(string name, string customBackgroundId, string customBackgroundId2, ElementStyle[] elementStyles)
        {
            Name = name;
            CustomBackgroundId = customBackgroundId;
            CustomBackgroundId2 = customBackgroundId2;
            ElementStyles = elementStyles.ToDictionary(x => x.Name, x => new MutableElementStyle(x));
        }

        public WindowStyle ToImmutable()
        {
            return new WindowStyle(Name, CustomBackgroundId, CustomBackgroundId2, ElementStyles.Values.Select(x => x.ToImmutable()).ToArray());
        }
    }
}
