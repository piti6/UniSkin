using System;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;


namespace UniSkin.UI
{
    internal readonly struct CachedInstructionInfo
    {
        public bool IsValid { get; }
        public SerializedObject StyleContainerSerializedObject { get; }
        public IReadOnlyDictionary<StyleStateType, SerializedProperty> StyleStateProperty { get; }
        public GUIStyleHolder StyleContainer { get; }

        public CachedInstructionInfo(GUIStyleHolder styleContainer)
        {
            IsValid = true;

            var serializedObject = new SerializedObject(styleContainer);

            StyleContainer = styleContainer;
            StyleContainerSerializedObject = serializedObject;
            StyleStateProperty = Enum.GetValues(typeof(StyleStateType))
                .Cast<StyleStateType>()
                .Select(type => (Type: type, Property: serializedObject.FindProperty($"inspectedStyle.m_{type}")))
                .ToDictionary(x => x.Type, x => x.Property);
        }
    }
}
