using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovingEnvironmentElement))]
public class MovingEnvironmentElementEditor : Editor
{
    private const float pointRadius = 0.2f;

    private readonly Color activeButtonColor = new Color(0.8f, 0.8f, 1);

    private MovingEnvironmentElement typedTarget;

    private SerializedProperty targetProperty;
    private SerializedProperty scriptProperty;
    private SerializedProperty pointAProperty;
    private SerializedProperty pointBProperty;
    private SerializedProperty movementTimeProperty;
    private SerializedProperty defaultStateProperty;
    private SerializedProperty arriveAtAProperty;
    private SerializedProperty arriveAtBProperty;
    private SerializedProperty startMovingToAProperty;
    private SerializedProperty startMovingToBProperty;

    private bool showPath = false;
    private bool updateA = false;
    private bool updateB = false;

    private void OnEnable()
    {
        typedTarget = target as MovingEnvironmentElement;

        scriptProperty = serializedObject.FindProperty("m_Script");
        targetProperty = serializedObject.FindProperty("target");
        pointAProperty = serializedObject.FindProperty("pointA");
        pointBProperty = serializedObject.FindProperty("pointB");
        movementTimeProperty = serializedObject.FindProperty("movementTime");
        defaultStateProperty = serializedObject.FindProperty("defaultState");
        arriveAtAProperty = serializedObject.FindProperty("arrivedAtA");
        arriveAtBProperty = serializedObject.FindProperty("arrivedAtB");
        startMovingToAProperty = serializedObject.FindProperty("startedMovingToA");
        startMovingToBProperty = serializedObject.FindProperty("startedMovingToB");

        showPath = false;
        updateA = false;
        updateB = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new GUIEnabledDisposable(false))
        {
            EditorGUILayout.PropertyField(scriptProperty);
        }

        EditorGUILayout.PropertyField(targetProperty);

        updateA = DrawPointProperty(pointAProperty, updateA);
        updateB = DrawPointProperty(pointBProperty, updateB);

        using (new HorizontalDisposable())
        {
            using (new GUIEnabledDisposable(targetProperty.objectReferenceValue != null))
            {
                if (GUILayout.Button("Move to A"))
                {
                    typedTarget.ToAImmiediately();
                }

                if (GUILayout.Button("Move to B"))
                {
                    typedTarget.ToBImmiediately();
                }
            }

            using (new GUIColorDisposable())
            {
                GUI.color = showPath ? activeButtonColor : GUI.color;

                if (GUILayout.Button("Show path"))
                {
                    showPath = !showPath;
                    SceneView.RepaintAll();
                }
            }
        }

        EditorGUILayout.PropertyField(movementTimeProperty);
        EditorGUILayout.PropertyField(defaultStateProperty);

        EditorGUILayout.PropertyField(arriveAtAProperty);
        EditorGUILayout.PropertyField(arriveAtBProperty);
        EditorGUILayout.PropertyField(startMovingToAProperty);
        EditorGUILayout.PropertyField(startMovingToBProperty);

        serializedObject.ApplyModifiedProperties();
    }

    private bool DrawPointProperty(SerializedProperty point, bool edit)
    {
        Rect pointARect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(point));
        Rect buttonARect = pointARect;
        buttonARect.height = EditorGUIUtility.singleLineHeight;
        buttonARect.xMin = pointARect.width * 0.75f;

        using (new GUIColorDisposable())
        {
            GUI.color = edit ? activeButtonColor : GUI.color;

            // button needs to be drawn first because unity handles events bottom to top
            if (GUI.Button(buttonARect, "Edit Point"))
            {
                edit = !edit;
                SceneView.RepaintAll();
            }
        }

        EditorGUI.PropertyField(pointARect, point, point.isExpanded);
        return edit;
    }

    private void OnSceneGUI()
    {
        if (showPath)
        {
            Vector3 localA = typedTarget.PointA.localPosition;
            Vector3 localB = typedTarget.PointB.localPosition;
            Vector3 aPosition = typedTarget.transform.TransformPoint(localA);
            Vector3 bPosition = typedTarget.transform.TransformPoint(localB);
            Handles.DrawLine(aPosition, bPosition);
        }

        if (updateA)
        {
            UpdatePoint(pointAProperty, typedTarget.transform);
        }

        if (updateB)
        {
            UpdatePoint(pointBProperty, typedTarget.transform);
        }

        if (updateA || updateB)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void UpdatePoint(SerializedProperty pointProperty, Transform transform)
    {
        using (new Handles.DrawingScope(Handles.color, Handles.matrix))
        {
            SerializedProperty localPositionProperty = pointProperty.FindPropertyRelative("localPosition");
            SerializedProperty localEulerProperty = pointProperty.FindPropertyRelative("localEuler");

            Vector3 localPosition = localPositionProperty.vector3Value;
            Vector3 position = transform.TransformPoint(localPosition);
            Quaternion localRotation = Quaternion.Euler(localEulerProperty.vector3Value);
            Quaternion rotation = transform.rotation * localRotation;

            Vector3 newPostition = Handles.PositionHandle(position, rotation);
            Vector3 newLocalPosition = transform.InverseTransformPoint(newPostition);

            Quaternion newRotation = Handles.RotationHandle(rotation, position);
            Quaternion newLocalRotation = Quaternion.Inverse(transform.rotation) * newRotation;

            localPositionProperty.vector3Value = newLocalPosition;
            localEulerProperty.vector3Value = newLocalRotation.eulerAngles;
        }
    }
}
