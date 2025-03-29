using UnityEngine;
using UnityEditor;
using System.Linq;
/// <summary>
/// Drawer for the RequireInterface attribute.
/// </summary>
[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    /// <summary>
    /// Overrides GUI drawing for the attribute.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="property">Property.</param>
    /// <param name="label">Label.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if this is reference type property.
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // Get attribute parameters.
            var requiredAttribute = this.attribute as RequireInterfaceAttribute;
            // Begin drawing property field.
            EditorGUI.BeginProperty(position, label, property);
            // Check for generic type
            if (requiredAttribute.requiredType.IsGenericType)
            {
                // Draw property field of standard interface type.
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(IGenericInterface), true);
                // Check for type in reference
                if (property.objectReferenceValue != null)
                {
                    if (property.objectReferenceValue.GetType().FindInterfaces(GenericInterfaceFilter, requiredAttribute.requiredType).Length <= 0)
                    {
                        // Null if type generic is not the same
                        property.objectReferenceValue = null;
                    }
                }
            }
            // Is not generic type
            else
            {
                // Draw property field.
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);
            }
            // Finish drawing property field.
            EditorGUI.EndProperty();
        }
        else
        {
            // If field is not reference, show error message.
            // Save previous color and change GUI to red.
            var previousColor = GUI.color;
            GUI.color = Color.red;
            // Display label with error message.
            EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
            // Revert color change.
            GUI.color = previousColor;
        }
    }

    private bool GenericInterfaceFilter(System.Type typeObj, System.Object criteriaObj)
    {
        if (typeObj.IsGenericType)
        {
            return typeObj.GetGenericTypeDefinition() == (System.Type)criteriaObj;
        }
        return typeObj == (System.Type)criteriaObj;
    }
}