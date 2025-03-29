using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InvokeButtonAttribute))]
public class InvokeButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.LabelField("bulech");
    }
}
