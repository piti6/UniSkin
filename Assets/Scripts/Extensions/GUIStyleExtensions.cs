using System.Collections.Generic;
using UnityEngine;

namespace UniSkin
{
    public static class GUIStyleExtensions
    {
        public static IEnumerable<(StyleStateType StyleStateType, GUIStyleState StyleState)> AsStyleStateEnumerable(this GUIStyle style)
        {
            yield return (StyleStateType.Normal, style.normal);
            yield return (StyleStateType.Active, style.active);
            yield return (StyleStateType.Focused, style.focused);
            yield return (StyleStateType.Hover, style.hover);
            yield return (StyleStateType.OnNormal, style.onNormal);
            yield return (StyleStateType.OnActive, style.onActive);
            yield return (StyleStateType.OnFocused, style.onFocused);
            yield return (StyleStateType.OnHover, style.onHover);
        }
    }
}
