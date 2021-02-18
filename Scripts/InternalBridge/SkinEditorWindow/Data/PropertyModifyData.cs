using UnityEngine;

namespace UniSkin.UI
{
    internal readonly struct PropertyModifyData
    {
        public string ModifiedElementStyleName { get; }
        public StyleState ModifiedStyleState { get; }
        public int FontSize { get; }
        public FontStyle FontStyle { get; }
        public string CustomBackgroundId { get; }
        public SerializableTexture2D AddedTexture { get; }

        public PropertyModifyData(string modifiedElementStyleName, int fontSize, FontStyle fontStyle)
            : this(modifiedElementStyleName, default, fontSize, fontStyle, string.Empty, default(SerializableTexture2D))
        { }

        public PropertyModifyData(string modifiedElementStyleName, string customBackgroundId, SerializableTexture2D addedTexture)
            : this(modifiedElementStyleName, default, default, default, customBackgroundId, addedTexture)
        { }

        public PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, SerializableTexture2D addedTexture)
            : this(modifiedElementStyleName, modifiedStyleState, default, default, default, addedTexture)
        { }

        private PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, int fontSize, FontStyle fontStyle, string customBackgroundId, SerializableTexture2D addedTexture)
        {
            ModifiedElementStyleName = modifiedElementStyleName;
            ModifiedStyleState = modifiedStyleState;
            FontSize = fontSize;
            FontStyle = fontStyle;
            CustomBackgroundId = customBackgroundId;
            AddedTexture = addedTexture;
        }
    }
}
