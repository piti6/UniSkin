using UnityEngine;

namespace UniSkin.UI
{
    internal readonly struct PropertyModifyData
    {
        public string ModifiedElementStyleName { get; }
        public StyleState ModifiedStyleState { get; }
        public int FontSize { get; }
        public FontStyle FontStyle { get; }
        public SerializableTexture2D AddedTexture { get; }

        public PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, int fontSize, FontStyle fontStyle, Texture2D addedTexture)
            : this(modifiedElementStyleName, modifiedStyleState, fontSize, fontStyle, addedTexture?.ToSerializableTexture2D())
        { }

        public PropertyModifyData(string modifiedElementStyleName, int fontSize, FontStyle fontStyle)
            : this(modifiedElementStyleName, default, fontSize, fontStyle, default(SerializableTexture2D))
        { }

        public PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, int fontSize, FontStyle fontStyle, SerializableTexture2D addedTexture)
        {
            ModifiedElementStyleName = modifiedElementStyleName;
            ModifiedStyleState = modifiedStyleState;
            FontSize = fontSize;
            FontStyle = fontStyle;
            AddedTexture = addedTexture;
        }
    }
}
