using System.Collections.Generic;
using System.Linq;

namespace UniSkin
{
    public class MutableWindowStyle
    {
        public string Name { get; set; }
        public Dictionary<string, MutableElementStyle> ElementStyles { get; set; }

        public MutableWindowStyle(WindowStyle windowStyle) : this(windowStyle.Name, windowStyle.ElementStyles.Values.ToArray())
        { }

        public MutableWindowStyle(string name, ElementStyle[] elementStyles)
        {
            Name = name;
            ElementStyles = elementStyles.ToDictionary(x => x.Name, x => new MutableElementStyle(x));
        }

        public WindowStyle ToImmutable()
        {
            return new WindowStyle(Name, ElementStyles.Values.Select(x => x.ToImmutable()).ToArray());
        }
    }
}
