using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScrollView
{
    private Vector2 scrollPosition;

    public void Begin()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
    }

    public void End()
    {
        EditorGUILayout.EndScrollView();
    }
}
