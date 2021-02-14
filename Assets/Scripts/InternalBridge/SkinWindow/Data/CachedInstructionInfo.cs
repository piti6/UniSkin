using System.Collections.Generic;
using UnityEditor;
using System.Linq;


namespace UniSkin.UI
{
    internal readonly struct CachedInstructionInfo
    {
        public bool IsValid { get; }
        public GUIStyleHolder StyleContainer { get; }
        public IReadOnlyDictionary<StyleStateType, StyleState> StyleStates { get; }

        public CachedInstructionInfo(GUIStyleHolder styleContainer)
        {
            IsValid = true;

            StyleContainer = styleContainer;
            StyleStates = styleContainer.inspectedStyle.AsStyleStateEnumerable()
                .ToDictionary(x => x.StyleStateType, x => x.StyleState.ToStyleState(x.StyleStateType));
        }
    }
}
