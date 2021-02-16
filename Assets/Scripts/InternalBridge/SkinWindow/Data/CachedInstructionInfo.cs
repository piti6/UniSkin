using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine;

namespace UniSkin.UI
{
    internal readonly struct CachedInstructionInfo
    {
        public bool IsValid { get; }
        public GUIStyleHolder StyleContainer { get; }
        public IReadOnlyDictionary<StyleStateType, StyleState> StyleStates { get; }
        public IReadOnlyDictionary<StyleStateType, IReadOnlyDictionary<string, Texture2D>> Textures { get; }
        public CachedInstructionInfo(GUIStyleHolder styleContainer)
        {
            IsValid = true;

            StyleContainer = styleContainer;
            StyleStates = styleContainer.inspectedStyle.AsStyleStateEnumerable()
                .ToDictionary(x => x.StyleStateType, x => x.StyleState.ToStyleState(x.StyleStateType));

            Textures = styleContainer.inspectedStyle.AsStyleStateEnumerable()
                .ToDictionary(x => x.StyleStateType, x => x.StyleState.scaledBackgrounds.Append(x.StyleState.background).Where(x => x != null).ToDictionary(x => x.ToTextureId()) as IReadOnlyDictionary<string, Texture2D>);
        }
    }
}
