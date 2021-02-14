using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin.UI
{
    internal readonly struct PropertyModifyData
    {
        public string ModifiedElementStyleName { get; }
        public StyleState ModifiedStyleState { get; }
        public int FontSize { get; }
        public FontStyle FontStyle { get; }
        public SerializableTexture2D[] AddedTextures { get; }

        public PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, int fontSize, FontStyle fontStyle, IEnumerable<Texture2D> addedTextures)
            : this(modifiedElementStyleName, modifiedStyleState, fontSize, fontStyle, addedTextures.Select(x => x.ToSerializableTexture2D()))
        { }

        public PropertyModifyData(string modifiedElementStyleName, int fontSize, FontStyle fontStyle)
            : this(modifiedElementStyleName, default, fontSize, fontStyle, Enumerable.Empty<SerializableTexture2D>())
        { }

        public PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, int fontSize, FontStyle fontStyle, IEnumerable<SerializableTexture2D> addedTextures)
        {
            ModifiedElementStyleName = modifiedElementStyleName;
            ModifiedStyleState = modifiedStyleState;
            FontSize = fontSize;
            FontStyle = fontStyle;
            AddedTextures = addedTextures.ToArray();
        }
    }
}
