using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagDropdownAttribute))]
public class TagDropdownDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUILayout.LabelField("Tag property must be of type string");
            return;
        }

        TagDropdownAttribute tagAttribute = attribute as TagDropdownAttribute;
        
        string currentValue = property.stringValue;
        string[] tags = InternalEditorUtility.tags;
        int idx = Array.IndexOf(tags, currentValue);
        idx = Mathf.Max(idx, 0);

        idx = EditorGUI.Popup(position, label.text, idx, tags);
        
        property.stringValue = tags[idx];
    }
}
