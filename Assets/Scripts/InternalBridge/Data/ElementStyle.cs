using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniSkin
{
    [Serializable]
    public class ElementStyle
    {
        public string Name => _name;
        public int FontSize => _fontSize;
        public FontStyle FontStyle => _fontStyle;
        public IReadOnlyDictionary<StyleStateType, StyleState> StyleStates => _styleStateDictionary ?? (_styleStateDictionary = _styleStates.ToDictionary(x => x.StateType));

        private IReadOnlyDictionary<StyleStateType, StyleState> _styleStateDictionary;

        [SerializeField]
        private string _name;
        [SerializeField]
        private int _fontSize;
        [SerializeField]
        private FontStyle _fontStyle;
        [SerializeField]
        private StyleState[] _styleStates;

        public ElementStyle() { }
        public ElementStyle(string name, int fontSize, FontStyle fontStyle, StyleState[] styleStates)
        {
            _name = name;
            _fontSize = fontSize;
            _fontStyle = fontStyle;
            _styleStates = styleStates;
        }
    }
}
