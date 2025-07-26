using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Linq;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(ActionBinding))]
    public class ActionBindingDrawer : StatefulPropertyDrawer<ActionBindingDrawer.State>
    {
        public class State
        {
            public int selectedBindingIndex = -1;
            public (string id, string name)[] bindingOptions = new (string id, string name)[0];
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            State state = GetState(property);

            SerializedProperty actionProperty = property.FindPropertyRelative("action");
            SerializedProperty bindingIdProperty = property.FindPropertyRelative("bindingId");

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(actionProperty);
                int newSelectedBindingIndex = EditorGUILayout.Popup("Binding", state.selectedBindingIndex, state.bindingOptions.Select(e => e.name).ToArray());

                if (newSelectedBindingIndex != state.selectedBindingIndex)
                {
                    string bindingId = state.bindingOptions[newSelectedBindingIndex].id;
                    bindingIdProperty.stringValue = bindingId;
                    state.selectedBindingIndex = newSelectedBindingIndex;
                }
            }

            RefreshBindingOptions(property, state);
        }

        protected override State CreateState(SerializedProperty property)
        {
            return new State();
        }

        // from unity samples
        private void RefreshBindingOptions(SerializedProperty property, State state)
        {
            SerializedProperty actionProperty = property.FindPropertyRelative("action");
            SerializedProperty bindingIdProperty = property.FindPropertyRelative("bindingId");

            InputActionReference actionReference = actionProperty.objectReferenceValue as InputActionReference;
            InputAction action = actionReference != null ? actionReference.action : null;

            if (action == null)
            {
                state.bindingOptions = new (string id, string name)[0];
                state.selectedBindingIndex = -1;
                return;
            }

            ReadOnlyArray<InputBinding> bindings = action.bindings;
            int bindingCount = bindings.Count;

            state.bindingOptions = new (string id, string name)[bindingCount];
            state.selectedBindingIndex = -1;

            string currentBindingId = bindingIdProperty.stringValue;
            for (int i = 0; i < bindingCount; ++i)
            {
                InputBinding binding = bindings[i];
                string bindingId = binding.id.ToString();
                bool haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                InputBinding.DisplayStringOptions displayOptions =
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
                if (!haveBindingGroups)
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

                // Create display string.
                string displayString = action.GetBindingDisplayString(i, displayOptions);

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (haveBindingGroups)
                {
                    InputActionAsset asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        string controlSchemes = string.Join(", ",
                            binding.groups.Split(InputBinding.Separator)
                                .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                state.bindingOptions[i] = (bindingId, displayString);

                if (currentBindingId == bindingId)
                    state.selectedBindingIndex = i;
            }
        }
    }
}