using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GUIStyleHolder))]
public class GUIStyleHolderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}
