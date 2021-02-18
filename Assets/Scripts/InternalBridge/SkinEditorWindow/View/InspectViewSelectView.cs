using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniSkin.UI
{
    internal class InspectViewSelectView
    {
        public event Action<object, string[], int> OnSelectInspectionValue = (userdata, options, selected) => { };

        public void Draw(GUIView currentInspectedView, Func<GUIView, bool> inspectableViewPredicator)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var selectedName = currentInspectedView is GUIView ? currentInspectedView.GetViewTitleName() : "<Please Select>";
                var selectedNameLabel = GUIContentUtility.UseCached(selectedName);

                GUILayout.Label("Inspected View: ", GUILayout.ExpandWidth(false));

                var popupPosition = GUILayoutUtility.GetRect(selectedNameLabel, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
                if (GUI.Button(popupPosition, selectedNameLabel, EditorStyles.toolbarDropDown))
                {
                    var views = new List<GUIView>();
                    GUIViewDebuggerHelper.GetViews(views);
                    views = views.Where(inspectableViewPredicator).ToList();

                    var options = views
                        .Select(x => x.GetViewTitleName())
                        .Prepend("None")
                        .Select(x => new GUIContent(x))
                        .ToArray();

                    var selectedIndex = views.IndexOf(currentInspectedView) + 1;

                    EditorUtility.DisplayCustomMenu(popupPosition, options, selectedIndex, OnSelectInspectionValue.Invoke, views);
                }
            }
        }
    }
}
