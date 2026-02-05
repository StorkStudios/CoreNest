using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(SubAssetAttribute))]
    public class SubAssetDrawer : PropertyDrawer
    {
        private static System.Type GetPropertyType(SerializedProperty property)
        {
            return property.GetFieldInfo()?.FieldType ?? property.GetArrayElementPropertyType();
        }

        private static bool IsPropertyTypeSupported(SerializedProperty property)
        {
            System.Type propertyType = GetPropertyType(property);
            bool propertyTypeCheck = property.propertyType == SerializedPropertyType.ObjectReference &&
                                     SubAssetUtils.CanTypeBeSubAsset(propertyType, true);

            Object targetObject = property.serializedObject.targetObject;
            bool isAssetCheck = !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(targetObject)) ||
                                PrefabUtility.IsPartOfAnyPrefab(targetObject);

            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            bool isInPrefabMode = stage != null && targetObject is Component c && c.gameObject.scene == stage.scene;
            isAssetCheck |= isInPrefabMode;

            return propertyTypeCheck && isAssetCheck;
        }

        private static string GetTargetAsset(Object target)
        {
            string path = AssetDatabase.GetAssetPath(target);
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null && target is Component c && c.gameObject.scene == stage.scene)
            {
                return stage.assetPath;
            }

            Object prefabInstance = PrefabUtility.GetOutermostPrefabInstanceRoot(target);
            path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabInstance);
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            return null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyTypeSupported(property))
            {
                string message = "SubAsset can only be used inside assets (.prefab, .asset, etc.) on fields of type UnityEngine.Object that are not components, game objects or shaders.";
                GUIContent content = EditorGUIUtility.IconContent("console.warnicon");
                content.text = message;
                content.tooltip = $"{property.serializedObject.targetObject.GetType().Name}/{property.propertyPath}";
                EditorGUI.LabelField(position, content, EditorStyles.wordWrappedLabel);
                return;
            }

            if (property.serializedObject.isEditingMultipleObjects)
            {
                // sub-assets are owned by a single object, so we can't handle multi-object editing
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Object targetObject = property.serializedObject.targetObject;
            string targetAssetPath = GetTargetAsset(targetObject);
            string targetAssetName = System.IO.Path.GetFileName(targetAssetPath);

            Object oldValue = property.objectReferenceValue;
            EditorGUI.BeginChangeCheck();
            
            GUIStyle buttonStyle = EditorStyles.iconButton;
            
            position.xMax -= buttonStyle.fixedWidth + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property, label);

            position.xMin = position.xMax + EditorGUIUtility.standardVerticalSpacing;
            position.xMax += buttonStyle.fixedWidth;
            position.yMin += 1;
            position.yMax = position.yMin + buttonStyle.fixedHeight;

            SubAssetManagerWindow.DrawSubAssetMenu(position, buttonStyle, property.objectReferenceValue, targetAssetPath, true, copy => property.objectReferenceValue = copy);

            if (EditorGUI.EndChangeCheck())
            {
                ReactToChanges(property, oldValue, targetAssetPath, targetAssetName);
            }
        }

        private void ReactToChanges(SerializedProperty property, Object oldValue, string targetAssetPath, string targetAssetName)
        {
            Object newValue = property.objectReferenceValue;
            if (newValue == oldValue)
            {
                return;
            }

            if (oldValue != null && SubAssetUtils.IsSubAssetOf(oldValue, targetAssetPath))
            {
                property.serializedObject.ApplyModifiedProperties();
                if (!AssetDatabase.LoadAllAssetsAtPath(targetAssetPath).Any(e => SubAssetUtils.IsSubAssetUsedInAsset(oldValue, e)))
                {
                    if (EditorUtility.DisplayDialog("SubAsset: Destroy sub-asset", $"Setting new value will make current sub-asset \"{targetAssetName}/{oldValue.name}\" unused. Consider making a copy and deleting it.", "Delete", "Leave as unused sub-asset"))
                    {
                        Object.DestroyImmediate(oldValue, true);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            if (newValue != null && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newValue)) && !AssetDatabase.IsSubAsset(newValue) && SubAssetUtils.CanInstanceBeSubAsset(newValue, targetAssetPath))
            {
                int choice = EditorUtility.DisplayDialogComplex("SubAsset: Create from existing", $"Assigned asset \"{newValue.name}\" can be made into a sub-asset of \"{targetAssetName}\"", "Make sub-asset", "Only set value", "Create sub-asset copy");
                switch (choice)
                {
                    case 0:
                        SubAssetUtils.MoveIntoSubAsset(newValue, targetAssetPath);
                        AssetDatabase.SaveAssets();
                        break;
                    case 2:
                        property.objectReferenceValue = SubAssetUtils.MakeSubAssetCopy(newValue, targetAssetPath);
                        AssetDatabase.SaveAssets();
                        break;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyTypeSupported(property))
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}