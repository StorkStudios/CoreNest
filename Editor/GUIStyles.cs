using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class GUIStyles
    {
        public const float listCountFieldWidth = 48;

        public readonly static GUIStyle Bold;

        public readonly static GUIStyle ListFoldout;

        public static Color BackgroundColor => EditorGUIUtility.isProSkin ? new Color(0.22f,0.22f,0.22f) : new Color(0.76f,0.76f,0.76f);
        public static Color HoverBackgroundColor => EditorGUIUtility.isProSkin ? new Color(0.27f, 0.27f, 0.27f) : new Color(0.82f, 0.82f, 0.82f);

        static GUIStyles ()
        {
            Bold = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
            };

            ListFoldout = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
            };
            ListFoldout.padding.left += 2;
            ListFoldout.margin.left -= 1;
        }
    }
}