using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            RequireInterfaceAttribute requiredAttribute = attribute as RequireInterfaceAttribute;
            System.Type requiredType = requiredAttribute.requiredType;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            Object value = property.objectReferenceValue;

            if (requiredType.IsGenericType && requiredType.ContainsGenericParameters)
            {
                value = EditorGUI.ObjectField(position, label, value, typeof(IGenericInterface), true);
                if (value != null)
                {
                    if (value.GetType().FindInterfaces(GenericInterfaceFilter, requiredType).Length <= 0)
                    {
                        value = null;
                    }
                }
            }
            else
            {
                value = EditorGUI.ObjectField(position, label, value, requiredType, true);
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = value;
            }

            EditorGUI.EndProperty();
        }
        else
        {
            using (new GUIColorDisposable())
            {
                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
            }
        }
    }

    private bool GenericInterfaceFilter(System.Type typeObj, object criteriaObj)
    {
        if (typeObj.IsGenericType)
        {
            return typeObj.GetGenericTypeDefinition() == (System.Type)criteriaObj;
        }
        return typeObj == (System.Type)criteriaObj;
    }
}