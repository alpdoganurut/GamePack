using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Shared.EditorDrawer
{
    [Serializable]
    public class CustomGUIStyle
    {
        private const string ValuesGetter = "GetAllBaseTypes";
        private const string RefreshStyleMethod = "RefreshStyle";

        
        [SerializeField, ValueDropdown(ValuesGetter), OnValueChanged(RefreshStyleMethod)] 
        private string _BaseStyleName;
        
        [SerializeField, OnValueChanged(RefreshStyleMethod)]
        private Color _NormalTextColor;
        
        [SerializeField, OnValueChanged(RefreshStyleMethod)]
        private Color _HoverTextColor;
        
        [SerializeField, OnValueChanged(RefreshStyleMethod)]
        private bool _StretchWidth;
        
        [SerializeField, OnValueChanged(RefreshStyleMethod)]
        private Font _Font;
        
        [SerializeField, OnValueChanged(RefreshStyleMethod)]
        private RectOffset _Margin;

        private bool _isInit;
        
        private GUIStyle _guiStyle;

        public GUIStyle GUIStyle
        {
            get
            {
                if(!_isInit) RefreshStyle();
                return _guiStyle;
            }
        }

        public CustomGUIStyle(string baseStyleName, Color normalTextColor, Color hoverTextColor, bool stretchWidth)
        {
            _BaseStyleName = baseStyleName;
            _NormalTextColor = normalTextColor;
            _HoverTextColor = hoverTextColor;
            _StretchWidth = stretchWidth;
            
            RefreshStyle();
        }

        [Button]
        public void RefreshStyle()
        {
            GUIStyle style;
            var propertyInfo = typeof(EditorStyles).GetProperty(_BaseStyleName, BindingFlags.Public | BindingFlags.Static);
            if (propertyInfo != null)
            {
                try
                {

                    var foundStyle = propertyInfo.GetValue(null) as GUIStyle;
                    style = new GUIStyle(foundStyle);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }
            else
            {
                Debug.LogError($"Can't find style named {_BaseStyleName}, creating empty style.");
                style = new GUIStyle();
            }

            style.normal.textColor = _NormalTextColor;
            style.hover.textColor = _HoverTextColor;
            style.stretchWidth = _StretchWidth;
            style.richText = true;
            style.font = _Font;
            style.margin = _Margin; 
            _guiStyle = style;
            
            _isInit = true;
        }

        private IEnumerable<string> GetAllBaseTypes()
        {
            var properties = typeof(EditorStyles).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var propertyInfo in properties)
            {
                yield return propertyInfo.Name;
            }
        }
    }
}