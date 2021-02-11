using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin.UI
{
    internal readonly struct PropertyModifyData
    {
        public string ModifiedElementStyleName { get; }
        public StyleState ModifiedStyleState { get; }
        public SerializableTexture2D[] AddedTextures { get; }

        public PropertyModifyData(string modifiedElementStyleName, StyleState modifiedStyleState, IEnumerable<Texture2D> addedTextures)
        {
            ModifiedElementStyleName = modifiedElementStyleName;
            ModifiedStyleState = modifiedStyleState;
            AddedTextures = addedTextures.Select(x => x.ToSerializableTexture2D()).ToArray();
        }
    }
}
