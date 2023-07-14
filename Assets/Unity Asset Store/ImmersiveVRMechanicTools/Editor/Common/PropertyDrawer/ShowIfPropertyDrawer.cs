using System.Collections.Generic;
using System.Reflection;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEditor;
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Editor.PropertyDrawer
{

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow((ShowIfAttribute)attribute, property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShow((ShowIfAttribute)attribute, property))
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                //The property is not being drawn
                //We want to undo the spacing added before and after the property
                return -EditorGUIUtility.standardVerticalSpacing;
                //return 0.0f;
            }
        }

        private static readonly Dictionary<string, PropertyInfo> _TypeFieldNameCombinationToPropertyInfoMap = new Dictionary<string, PropertyInfo>();
        private bool ShouldShow(ShowIfAttribute showIfAttribute, SerializedProperty property)
        {
            var checkFieldName = showIfAttribute.CheckFieldName;
            var serializedFieldObject = property.serializedObject.FindProperty(checkFieldName);
            if (serializedFieldObject != null)
            {
                return serializedFieldObject.boolValue;
            }
            else
            {
                var targetType = property.serializedObject.targetObject.GetType();
                var typeFieldNameCombination = $"{targetType.FullName}.{checkFieldName}";

                PropertyInfo propertyInfo;
                if (!_TypeFieldNameCombinationToPropertyInfoMap.ContainsKey(typeFieldNameCombination))
                {
                    propertyInfo = property.serializedObject
                        .targetObject.GetType()
                        .GetProperty(showIfAttribute.CheckFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if (propertyInfo == null)
                    {
                        UnityEngine.Debug.LogWarning($"Unable to find {nameof(PropertyInfo)} for '{typeFieldNameCombination}'");
                        return true;
                    }

                    _TypeFieldNameCombinationToPropertyInfoMap[typeFieldNameCombination] = propertyInfo;
                }
                else
                {
                    propertyInfo = _TypeFieldNameCombinationToPropertyInfoMap[typeFieldNameCombination];
                }

                var value = propertyInfo.GetValue(property.serializedObject.targetObject);
                if (value is bool bValue)
                {
                    return bValue;
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Unable to get value as bool for '{typeFieldNameCombination}'");
                    return true;
                }
            }
        }
    }
}
