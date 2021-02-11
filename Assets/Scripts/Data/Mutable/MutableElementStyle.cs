using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    public class MutableElementStyle
    {
        public string Name { get; set; }
        public int FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public Dictionary<StyleStateType, MutableStyleState> StyleStates { get; set; }

        public MutableElementStyle()
        {
            StyleStates = new Dictionary<StyleStateType, MutableStyleState>();
        }

        public MutableElementStyle(ElementStyle elementStyle) : this(elementStyle.Name, elementStyle.FontSize, elementStyle.FontStyle, elementStyle.StyleStates.Values.ToArray())
        { }

        public MutableElementStyle(string name, int fontSize, FontStyle fontStyle, StyleState[] styleStates)
        {
            Name = name;
            FontSize = fontSize;
            FontStyle = fontStyle;
            StyleStates = styleStates.ToDictionary(x => x.StateType, x => new MutableStyleState(x));
        }

        public ElementStyle ToImmutable()
        {
            return new ElementStyle(Name, FontSize, FontStyle, StyleStates.Values.Select(x => x.ToImmutable()).ToArray());
        }
    }
}
