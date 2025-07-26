using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    // modified easy event editor https://github.com/MerlinVR/EasyEventEditor
    [CustomPropertyDrawer(typeof(UnityEvent))]
    public class UnityEventDrawer : PropertyDrawer
    {
        private static class Settings
        {
            public static bool showPrivateMembers = true;
            public static bool showInvokeField = true;
            public static bool displayArgumentType = true;
            public static bool groupSameComponentType = false;
        }

        class DrawerState
        {
            public ReorderableList reorderableList;
            public int lastSelectedIndex;

            // Invoke field tracking
            public string currentInvokeStrArg = "";
            public int currentInvokeIntArg = 0;
            public float currentInvokeFloatArg = 0f;
            public bool currentInvokeBoolArg = false;
            public Object currentInvokeObjectArg = null;
        }

        class FunctionData
        {
            public FunctionData(SerializedProperty listener, Object target = null, MethodInfo method = null, PersistentListenerMode mode = PersistentListenerMode.EventDefined)
            {
                listenerElement = listener;
                targetObject = target;
                targetMethod = method;
                listenerMode = mode;
            }

            public SerializedProperty listenerElement;
            public Object targetObject;
            public MethodInfo targetMethod;
            public PersistentListenerMode listenerMode;
        }

        class ComponentTypeCount
        {
            public int TotalCount = 0;
            public int CurrentCount = 1;
        }

        Dictionary<string, DrawerState> drawerStates = new Dictionary<string, DrawerState>();

        DrawerState currentState;
        string currentLabelText;
        SerializedProperty currentProperty;
        SerializedProperty listenerArray;

        UnityEventBase dummyEvent;
        MethodInfo cachedFindMethodInfo = null;

        public static System.Type FindTypeInAllAssemblies(string qualifiedTypeName)
        {
            System.Type t = System.Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (System.Reflection.Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }

                return null;
            }
        }

        private static UnityEventBase GetDummyEventStep(string propertyPath, System.Type propertyType, BindingFlags bindingFlags)
        {
            UnityEventBase dummyEvent = null;

            while (propertyPath.Length > 0)
            {
                if (propertyPath.StartsWith("."))
                    propertyPath = propertyPath.Substring(1);

                string[] splitPath = propertyPath.Split(new char[] { '.' }, 2);

                FieldInfo newField = propertyType.GetField(splitPath[0], bindingFlags);

                if (newField == null)
                    break;

                propertyType = newField.FieldType;
                if (propertyType.IsArray)
                {
                    propertyType = propertyType.GetElementType();
                }
                else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    propertyType = propertyType.GetGenericArguments()[0];
                }

                if (splitPath.Length == 1)
                    break;

                propertyPath = splitPath[1];
                if (propertyPath.StartsWith("Array.data["))
                    propertyPath = propertyPath.Split(new char[] { ']' }, 2)[1];
            }

            if (propertyType.IsSubclassOf(typeof(UnityEventBase)))
                dummyEvent = System.Activator.CreateInstance(propertyType) as UnityEventBase;

            return dummyEvent;
        }

        private static UnityEventBase GetDummyEvent(SerializedProperty property)
        {
            Object targetObject = property.serializedObject.targetObject;
            if (targetObject == null)
                return new UnityEvent();

            UnityEventBase dummyEvent = null;
            System.Type targetType = targetObject.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            do
            {
                dummyEvent = GetDummyEventStep(property.propertyPath, targetType, bindingFlags);
                bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                targetType = targetType.BaseType;
            } while (dummyEvent == null && targetType != null);

            return dummyEvent ?? new UnityEvent();
        }

        static void InvokeOnTargetEvents(MethodInfo method, object[] targets, object argValue)
        {
            foreach (object target in targets)
            {
                if (argValue != null)
                    method.Invoke(target, new object[] { argValue });
                else
                    method.Invoke(target, new object[] { });
            }
        }

        static void SetEventFunctionCallback(object functionUserData)
        {
            FunctionData functionData = functionUserData as FunctionData;

            SerializedProperty serializedElement = functionData.listenerElement;

            SerializedProperty serializedTarget = serializedElement.FindPropertyRelative("m_Target");
            SerializedProperty serializedMethodName = serializedElement.FindPropertyRelative("m_MethodName");
            SerializedProperty serializedArgs = serializedElement.FindPropertyRelative("m_Arguments");
            SerializedProperty serializedMode = serializedElement.FindPropertyRelative("m_Mode");

            SerializedProperty serializedArgAssembly = serializedArgs.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
            SerializedProperty serializedArgObjectValue = serializedArgs.FindPropertyRelative("m_ObjectArgument");

            serializedTarget.objectReferenceValue = functionData.targetObject;
            serializedMethodName.stringValue = functionData.targetMethod.Name;
            if (serializedMode.type == "int")
            {
                serializedMode.intValue = (int)functionData.listenerMode;
            }
            else
            {
                serializedMode.enumValueIndex = (int)functionData.listenerMode;
            }

            if (functionData.listenerMode == PersistentListenerMode.Object)
            {
                ParameterInfo[] methodParams = functionData.targetMethod.GetParameters();
                if (methodParams.Length == 1 && typeof(Object).IsAssignableFrom(methodParams[0].ParameterType))
                    serializedArgAssembly.stringValue = methodParams[0].ParameterType.AssemblyQualifiedName;
                else
                    serializedArgAssembly.stringValue = typeof(Object).AssemblyQualifiedName;
            }
            else
            {
                serializedArgAssembly.stringValue = typeof(Object).AssemblyQualifiedName;
                serializedArgObjectValue.objectReferenceValue = null;
            }

            System.Type argType = FindTypeInAllAssemblies(serializedArgAssembly.stringValue);
            if (!typeof(Object).IsAssignableFrom(argType) || !argType.IsInstanceOfType(serializedArgObjectValue.objectReferenceValue))
                serializedArgObjectValue.objectReferenceValue = null;

            functionData.listenerElement.serializedObject.ApplyModifiedProperties();
        }

        static void ClearEventFunctionCallback(object functionUserData)
        {
            FunctionData functionData = functionUserData as FunctionData;

            SerializedProperty serializedMode = functionData.listenerElement.FindPropertyRelative("m_Mode");
            if (serializedMode.type == "int")
            {
                serializedMode.intValue = (int)PersistentListenerMode.Void;
            }
            else
            {
                serializedMode.enumValueIndex = (int)PersistentListenerMode.Void;
            }

            functionData.listenerElement.FindPropertyRelative("m_MethodName").stringValue = null;
            functionData.listenerElement.serializedObject.ApplyModifiedProperties();
        }

        MethodInfo InvokeFindMethod(string functionName, object targetObject, UnityEventBase eventObject, PersistentListenerMode listenerMode, System.Type argType = null)
        {
            MethodInfo findMethod = cachedFindMethodInfo;

            if (findMethod == null)
            {
                // Rather not reinvent the wheel considering this function calls different functions depending on the number of args the event has...
                findMethod = eventObject.GetType().GetMethod("FindMethod", BindingFlags.NonPublic | BindingFlags.Instance, null,
                        new System.Type[] {
                    typeof(string),
                    typeof(System.Type),
                    typeof(PersistentListenerMode),
                    typeof(System.Type)
                        },
                    null);

                cachedFindMethodInfo = findMethod;
            }

            if (findMethod == null)
            {
                Debug.LogError("Could not find FindMethod function!");
                return null;
            }
            return findMethod.Invoke(eventObject, new object[] { functionName, targetObject?.GetType(), listenerMode, argType }) as MethodInfo;
        }

        System.Type[] GetEventParams(UnityEventBase eventIn)
        {
            MethodInfo methodInfo = InvokeFindMethod("Invoke", eventIn, eventIn, PersistentListenerMode.EventDefined);
            return methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
        }

        string GetEventParamsStr(UnityEventBase eventIn)
        {
            StringBuilder builder = new StringBuilder();
            System.Type[] methodTypes = GetEventParams(eventIn);

            builder.Append("(");
            builder.Append(string.Join(", ", methodTypes.Select(val => val.Name).ToArray()));
            builder.Append(")");

            return builder.ToString();
        }

        Rect[] GetElementRects(Rect rect)
        {
            Rect[] rects = new Rect[4];

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;

            // enabled field
            rects[0] = rect;
            rects[0].width *= 0.3f;

            // game object field
            rects[1] = rects[0];
            rects[1].x += 1;
            rects[1].width -= 2;
            rects[1].y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // function field
            rects[2] = rect;
            rects[2].xMin = rects[1].xMax + 5;

            // argument field
            rects[3] = rects[2];
            rects[3].y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return rects;
        }

        string GetTypeName(System.Type typeToName)
        {
            if (typeToName == typeof(float))
                return "float";
            if (typeToName == typeof(bool))
                return "bool";
            if (typeToName == typeof(int))
                return "int";
            if (typeToName == typeof(string))
                return "string";

            return typeToName.Name;
        }

        string GetFunctionArgStr(string functionName, object targetObject, PersistentListenerMode listenerMode, System.Type argType = null)
        {
            MethodInfo methodInfo = InvokeFindMethod(functionName, targetObject, dummyEvent, listenerMode, argType);

            if (methodInfo == null)
                return "";

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length == 0)
                return "";

            return GetTypeName(parameterInfos[0].ParameterType);
        }

        string GetFunctionDisplayName(SerializedProperty objectProperty, SerializedProperty methodProperty, PersistentListenerMode listenerMode, System.Type argType, bool showArg)
        {
            string methodNameOut = "No Function";

            if (objectProperty.objectReferenceValue == null || methodProperty.stringValue == "")
                return methodNameOut;

            MethodInfo methodInfo = InvokeFindMethod(methodProperty.stringValue, objectProperty.objectReferenceValue, dummyEvent, listenerMode, argType);
            string funcName = methodProperty.stringValue.StartsWith("set_") ? methodProperty.stringValue.Substring(4) : methodProperty.stringValue;

            if (methodInfo == null)
            {
                methodNameOut = string.Format("<Missing {0}.{1}>", objectProperty.objectReferenceValue.GetType().Name.ToString(), funcName);
                return methodNameOut;
            }

            string objectTypeName = objectProperty.objectReferenceValue.GetType().Name;
            Component objectComponent = objectProperty.objectReferenceValue as Component;

            if (!Settings.groupSameComponentType && objectComponent != null)
            {
                System.Type objectType = objectProperty.objectReferenceValue.GetType();

                Component[] components = objectComponent.GetComponents(objectType);

                if (components.Length > 1)
                {
                    int componentID = 0;
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == objectComponent)
                        {
                            componentID = i + 1;
                            break;
                        }
                    }

                    objectTypeName += string.Format("({0})", componentID);
                }
            }

            if (showArg)
            {
                string functionArgStr = GetFunctionArgStr(methodProperty.stringValue, objectProperty.objectReferenceValue, listenerMode, argType);
                methodNameOut = string.Format("{0}.{1} ({2})", objectTypeName, funcName, functionArgStr);
            }
            else
            {
                methodNameOut = string.Format("{0}.{1}", objectTypeName, funcName);
            }


            return methodNameOut;
        }

        System.Type[] GetTypeForListenerMode(PersistentListenerMode listenerMode)
        {
            switch (listenerMode)
            {
                case PersistentListenerMode.EventDefined:
                case PersistentListenerMode.Void:
                    return new System.Type[] { };
                case PersistentListenerMode.Object:
                    return new System.Type[] { typeof(Object) };
                case PersistentListenerMode.Int:
                    return new System.Type[] { typeof(int) };
                case PersistentListenerMode.Float:
                    return new System.Type[] { typeof(float) };
                case PersistentListenerMode.String:
                    return new System.Type[] { typeof(string) };
                case PersistentListenerMode.Bool:
                    return new System.Type[] { typeof(bool) };
            }

            return new System.Type[] { };
        }

        void FindValidMethods(Object targetObject, PersistentListenerMode listenerMode, List<FunctionData> methodInfos, System.Type[] customArgTypes = null)
        {
            System.Type objectType = targetObject.GetType();

            System.Type[] argTypes;

            if (listenerMode == PersistentListenerMode.EventDefined && customArgTypes != null)
                argTypes = customArgTypes;
            else
                argTypes = GetTypeForListenerMode(listenerMode);

            List<MethodInfo> foundMethods = new List<MethodInfo>();

            // For some reason BindingFlags.FlattenHierarchy does not seem to work, so we manually traverse the base types instead
            while (objectType != null)
            {
                MethodInfo[] foundMethodsOnType = objectType.GetMethods(BindingFlags.Public | (Settings.showPrivateMembers ? BindingFlags.NonPublic : BindingFlags.Default) | BindingFlags.Instance);

                foundMethods.AddRange(foundMethodsOnType);

                objectType = objectType.BaseType;
            }

            foreach (MethodInfo methodInfo in foundMethods)
            {
                // Sadly we can only use functions with void return type since C# throws an error
                if (methodInfo.ReturnType != typeof(void))
                    continue;

                ParameterInfo[] methodParams = methodInfo.GetParameters();
                if (methodParams.Length != argTypes.Length)
                    continue;

                bool isValidParamMatch = true;
                for (int i = 0; i < methodParams.Length; i++)
                {
                    if (!methodParams[i].ParameterType.IsAssignableFrom(argTypes[i])/* && (argTypes[i] != typeof(int) || !methodParams[i].ParameterType.IsEnum)*/)
                    {
                        isValidParamMatch = false;
                    }
                    if (listenerMode == PersistentListenerMode.Object && argTypes[i].IsAssignableFrom(methodParams[i].ParameterType))
                    {
                        isValidParamMatch = true;
                    }
                }

                if (!isValidParamMatch)
                    continue;

                if (!Settings.showPrivateMembers && methodInfo.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).Length > 0)
                    continue;


                FunctionData foundMethodData = new FunctionData(null, targetObject, methodInfo, listenerMode);

                methodInfos.Add(foundMethodData);
            }
        }

        void AddFunctionToMenu(string contentPath, SerializedProperty elementProperty, FunctionData methodData, GenericMenu menu, int componentCount, bool dynamicCall = false)
        {
            string functionName = (methodData.targetMethod.Name.StartsWith("set_") ? methodData.targetMethod.Name.Substring(4) : methodData.targetMethod.Name);
            string argStr = string.Join(", ", methodData.targetMethod.GetParameters().Select(param => GetTypeName(param.ParameterType)).ToArray());

            if (dynamicCall) // Cut out the args from the dynamic variation to match Unity, and the menu item won't be created if it's not unique.
            {
                contentPath += functionName;
            }
            else
            {
                if (methodData.targetMethod.Name.StartsWith("set_")) // If it's a property add the arg before the name
                {
                    contentPath += argStr + " " + functionName;
                }
                else
                {
                    contentPath += functionName + " (" + argStr + ")"; // Add arguments
                }
            }

            if (!methodData.targetMethod.IsPublic)
                contentPath += " " + (methodData.targetMethod.IsPrivate ? "<private>" : "<internal>");

            if (methodData.targetMethod.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).Length > 0)
                contentPath += " <obsolete>";

            methodData.listenerElement = elementProperty;

            SerializedProperty serializedTargetObject = elementProperty.FindPropertyRelative("m_Target");
            SerializedProperty serializedMethodName = elementProperty.FindPropertyRelative("m_MethodName");
            SerializedProperty serializedMode = elementProperty.FindPropertyRelative("m_Mode");

            bool isModeCorrect = serializedMode.type switch
            {
                "int" => serializedMode.intValue == (int)methodData.listenerMode,
                _ => serializedMode.enumValueIndex == (int)methodData.listenerMode
            };

            bool itemOn = serializedTargetObject.objectReferenceValue == methodData.targetObject &&
                          serializedMethodName.stringValue == methodData.targetMethod.Name &&
                          isModeCorrect;

            menu.AddItem(new GUIContent(contentPath), itemOn, SetEventFunctionCallback, methodData);
        }

        void BuildMenuForObject(Object targetObject, SerializedProperty elementProperty, GenericMenu menu, int componentCount = 0)
        {
            List<FunctionData> methodInfos = new List<FunctionData>();
            string contentPath = targetObject.GetType().Name + (componentCount > 0 ? string.Format("({0})", componentCount) : "") + "/";

            FindValidMethods(targetObject, PersistentListenerMode.Void, methodInfos);
            FindValidMethods(targetObject, PersistentListenerMode.Int, methodInfos);
            FindValidMethods(targetObject, PersistentListenerMode.Float, methodInfos);
            FindValidMethods(targetObject, PersistentListenerMode.String, methodInfos);
            FindValidMethods(targetObject, PersistentListenerMode.Bool, methodInfos);
            FindValidMethods(targetObject, PersistentListenerMode.Object, methodInfos);

            methodInfos = methodInfos.OrderBy(method1 => method1.targetMethod.Name.StartsWith("set_") ? 0 : 1).ThenBy((method1) => method1.targetMethod.Name).ToList();

            // Get event args to determine if we can do a pass through of the arg to the parameter
            System.Type[] eventArgs = dummyEvent.GetType().GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();

            bool dynamicBinding = false;

            if (eventArgs.Length > 0)
            {
                List<FunctionData> dynamicMethodInfos = new List<FunctionData>();
                FindValidMethods(targetObject, PersistentListenerMode.EventDefined, dynamicMethodInfos, eventArgs);

                if (dynamicMethodInfos.Count > 0)
                {
                    dynamicMethodInfos = dynamicMethodInfos.OrderBy(m => m.targetMethod.Name.StartsWith("set") ? 0 : 1).ThenBy(m => m.targetMethod.Name).ToList();

                    dynamicBinding = true;

                    // Add dynamic header
                    menu.AddDisabledItem(new GUIContent(contentPath + string.Format("Dynamic {0}", GetTypeName(eventArgs[0]))));
                    menu.AddSeparator(contentPath);

                    foreach (FunctionData dynamicMethod in dynamicMethodInfos)
                    {
                        AddFunctionToMenu(contentPath, elementProperty, dynamicMethod, menu, 0, true);
                    }
                }
            }

            // Add static header if we have dynamic bindings
            if (dynamicBinding)
            {
                menu.AddDisabledItem(new GUIContent(contentPath + "Static Parameters"));
                menu.AddSeparator(contentPath);
            }

            foreach (FunctionData method in methodInfos)
            {
                AddFunctionToMenu(contentPath, elementProperty, method, menu, componentCount);
            }
        }

        GenericMenu BuildPopupMenu(Object targetObj, SerializedProperty elementProperty, System.Type objectArgType)
        {
            GenericMenu menu = new GenericMenu();

            string currentMethodName = elementProperty.FindPropertyRelative("m_MethodName").stringValue;

            menu.AddItem(new GUIContent("No Function"), string.IsNullOrEmpty(currentMethodName), ClearEventFunctionCallback, new FunctionData(elementProperty));
            menu.AddSeparator("");

            if (targetObj is Component)
            {
                targetObj = (targetObj as Component).gameObject;
            }
            else if (!(targetObj is GameObject))
            {
                // Function menu for asset objects and such
                BuildMenuForObject(targetObj, elementProperty, menu);
                return menu;
            }

            // GameObject menu
            BuildMenuForObject(targetObj, elementProperty, menu);

            IEnumerable<Component> components = (targetObj as GameObject).GetComponents<Component>().Where(component => component != null);
            Dictionary<System.Type, ComponentTypeCount> componentTypeCounts = new Dictionary<System.Type, ComponentTypeCount>();

            // Only get the first instance of each component type
            if (Settings.groupSameComponentType)
            {
                components = components.GroupBy(comp => comp.GetType()).Select(group => group.First());
            }
            else // Otherwise we need to know if there are multiple components of a given type before we start going through the components since we only need numbers on component types with multiple instances.
            {
                foreach (Component component in components)
                {
                    if (!componentTypeCounts.TryGetValue(component.GetType(), out ComponentTypeCount typeCount))
                    {
                        typeCount = new ComponentTypeCount();
                        componentTypeCounts.Add(component.GetType(), typeCount);
                    }

                    typeCount.TotalCount++;
                }

            }

            foreach (Component component in components)
            {
                int componentCount = 0;

                if (!Settings.groupSameComponentType)
                {
                    ComponentTypeCount typeCount = componentTypeCounts[component.GetType()];
                    if (typeCount.TotalCount > 1)
                        componentCount = typeCount.CurrentCount++;
                }

                BuildMenuForObject(component, elementProperty, menu, componentCount);
            }

            return menu;
        }

        void ResetEventState(SerializedProperty serialiedListener)
        {
            SerializedProperty serializedCallState = serialiedListener.FindPropertyRelative("m_CallState");
            SerializedProperty serializedTarget = serialiedListener.FindPropertyRelative("m_Target");
            SerializedProperty serializedMethodName = serialiedListener.FindPropertyRelative("m_MethodName");
            SerializedProperty serializedMode = serialiedListener.FindPropertyRelative("m_Mode");
            SerializedProperty serializedArgs = serialiedListener.FindPropertyRelative("m_Arguments");

            if (serializedCallState.type == "int")
            {
                serializedCallState.intValue = (int)UnityEventCallState.RuntimeOnly;
            }
            else
            {
                serializedCallState.enumValueIndex = (int)UnityEventCallState.RuntimeOnly;
            }
            serializedTarget.objectReferenceValue = null;
            serializedMethodName.stringValue = null;
            if (serializedMode.type == "int")
            {
                serializedMode.intValue = (int)PersistentListenerMode.Void;
            }
            else
            {
                serializedMode.enumValueIndex = (int)PersistentListenerMode.Void;
            }

            serializedArgs.FindPropertyRelative("m_IntArgument").intValue = 0;
            serializedArgs.FindPropertyRelative("m_FloatArgument").floatValue = 0f;
            serializedArgs.FindPropertyRelative("m_BoolArgument").boolValue = false;
            serializedArgs.FindPropertyRelative("m_StringArgument").stringValue = null;
            serializedArgs.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = null;
            serializedArgs.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = null;
        }

        private void PrepareState(SerializedProperty propertyForState)
        {
            DrawerState state;

            if (!drawerStates.TryGetValue(propertyForState.propertyPath, out state))
            {
                state = new DrawerState();

                SerializedProperty persistentListeners = propertyForState.FindPropertyRelative("m_PersistentCalls.m_Calls");

                // The fun thing is that if Unity just made the first bool arg true internally, this whole thing would be unnecessary.
                state.reorderableList = new ReorderableList(propertyForState.serializedObject, persistentListeners, true, true, true, true);
                state.reorderableList.elementHeight = 43; // todo: actually find proper constant for this. 
                state.reorderableList.drawHeaderCallback += DrawHeaderCallback;
                state.reorderableList.drawElementCallback += DrawElementCallback;
                state.reorderableList.onSelectCallback += SelectCallback;
                state.reorderableList.onRemoveCallback += ReorderCallback;
                state.reorderableList.onAddCallback += AddEventListener;
                state.reorderableList.onRemoveCallback += RemoveCallback;

                state.lastSelectedIndex = 0;

                drawerStates.Add(propertyForState.propertyPath, state);
            }

            currentProperty = propertyForState;

            currentState = state;
            currentState.reorderableList.index = currentState.lastSelectedIndex;
            listenerArray = state.reorderableList.serializedProperty;

            dummyEvent = GetDummyEvent(propertyForState);
        }

        void DrawInvokeField(Rect position, float headerStartOffset)
        {
            Rect buttonPos = position;
            buttonPos.height *= 0.9f;
            buttonPos.width = 51;
            buttonPos.x += headerStartOffset + 2;

            Rect textPos = buttonPos;
            textPos.x += 6;
            textPos.width -= 12;

            Rect inputFieldPos = position;
            inputFieldPos.height = buttonPos.height;
            inputFieldPos.width = position.width - buttonPos.width - 3 - headerStartOffset;
            inputFieldPos.x = buttonPos.x + buttonPos.width + 2;
            inputFieldPos.y += 1;

            Rect inputFieldTextPlaceholder = inputFieldPos;

            System.Type[] eventInvokeArgs = GetEventParams(dummyEvent);

            GUIStyle textStyle = EditorStyles.miniLabel;
            textStyle.alignment = TextAnchor.MiddleLeft;

            MethodInfo invokeMethod = InvokeFindMethod("Invoke", dummyEvent, dummyEvent, PersistentListenerMode.EventDefined);
            FieldInfo serializedField = currentProperty.serializedObject.targetObject.GetType().GetField(currentProperty.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            object[] invokeTargets = currentProperty.serializedObject.targetObjects.Select(target => target == null || serializedField == null ? null : serializedField.GetValue(target)).Where(f => f != null).ToArray();

            EditorGUI.BeginDisabledGroup(invokeTargets.Length == 0 || invokeMethod == null);

            bool executeInvoke = GUI.Button(buttonPos, "", EditorStyles.miniButton);
            GUI.Label(textPos, "Invoke"/* + " (" + string.Join(", ", eventInvokeArgs.Select(e => e.Name).ToArray()) + ")"*/, textStyle);

            if (eventInvokeArgs.Length > 0)
            {
                System.Type argType = eventInvokeArgs[0];

                if (argType == typeof(string))
                {
                    currentState.currentInvokeStrArg = EditorGUI.TextField(inputFieldPos, currentState.currentInvokeStrArg);

                    // Draw placeholder text
                    if (currentState.currentInvokeStrArg.Length == 0)
                    {
                        GUIStyle placeholderLabelStyle = EditorStyles.centeredGreyMiniLabel;
                        placeholderLabelStyle.alignment = TextAnchor.UpperLeft;

                        GUI.Label(inputFieldTextPlaceholder, "String argument...", placeholderLabelStyle);
                    }

                    if (executeInvoke)
                        InvokeOnTargetEvents(invokeMethod, invokeTargets, currentState.currentInvokeStrArg);
                }
                else if (argType == typeof(int))
                {
                    currentState.currentInvokeIntArg = EditorGUI.IntField(inputFieldPos, currentState.currentInvokeIntArg);

                    if (executeInvoke)
                        InvokeOnTargetEvents(invokeMethod, invokeTargets, currentState.currentInvokeIntArg);
                }
                else if (argType == typeof(float))
                {
                    currentState.currentInvokeFloatArg = EditorGUI.FloatField(inputFieldPos, currentState.currentInvokeFloatArg);

                    if (executeInvoke)
                        InvokeOnTargetEvents(invokeMethod, invokeTargets, currentState.currentInvokeFloatArg);
                }
                else if (argType == typeof(bool))
                {
                    currentState.currentInvokeBoolArg = EditorGUI.Toggle(inputFieldPos, currentState.currentInvokeBoolArg);

                    if (executeInvoke)
                        InvokeOnTargetEvents(invokeMethod, invokeTargets, currentState.currentInvokeBoolArg);
                }
                else if (argType == typeof(Object))
                {
                    currentState.currentInvokeObjectArg = EditorGUI.ObjectField(inputFieldPos, currentState.currentInvokeObjectArg, argType, true);

                    if (executeInvoke)
                        invokeMethod.Invoke(currentProperty.serializedObject.targetObject, new object[] { currentState.currentInvokeObjectArg });
                }
            }
            else if (executeInvoke) // No input arg
            {
                InvokeOnTargetEvents(invokeMethod, invokeTargets, null);
            }

            EditorGUI.EndDisabledGroup();
        }

        void DrawHeaderCallback(Rect headerRect)
        {
            // We need to know where to position the invoke field based on the length of the title in the UI
            GUIContent headerTitle = new GUIContent(string.IsNullOrEmpty(currentLabelText) ? "Event" : currentLabelText + " " + GetEventParamsStr(dummyEvent));
            float headerStartOffset = EditorStyles.label.CalcSize(headerTitle).x;

            GUI.Label(headerRect, headerTitle);

            if (Settings.showInvokeField)
                DrawInvokeField(headerRect, headerStartOffset);
        }

        void DrawElementCallback(Rect rect, int index, bool active, bool focused)
        {
            SerializedProperty element = listenerArray.GetArrayElementAtIndex(index);

            rect.y++;
            Rect[] rects = GetElementRects(rect);

            Rect enabledRect = rects[0];
            Rect gameObjectRect = rects[1];
            Rect functionRect = rects[2];
            Rect argRect = rects[3];

            SerializedProperty serializedCallState = element.FindPropertyRelative("m_CallState");
            SerializedProperty serializedMode = element.FindPropertyRelative("m_Mode");
            SerializedProperty serializedArgs = element.FindPropertyRelative("m_Arguments");
            SerializedProperty serializedTarget = element.FindPropertyRelative("m_Target");
            SerializedProperty serializedMethod = element.FindPropertyRelative("m_MethodName");

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;

            // fix for the enum type loss
            if (serializedCallState.type == "int")
            {
                serializedCallState.intValue = System.Convert.ToInt32(EditorGUI.EnumPopup(enabledRect, GUIContent.none, (UnityEventCallState)serializedCallState.intValue));
            }
            else
            {
                EditorGUI.PropertyField(enabledRect, serializedCallState, GUIContent.none);
            }

            EditorGUI.BeginChangeCheck();

            Object oldTargetObject = serializedTarget.objectReferenceValue;

            GUI.Box(gameObjectRect, GUIContent.none);
            EditorGUI.PropertyField(gameObjectRect, serializedTarget, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                Object newTargetObject = serializedTarget.objectReferenceValue;

                // Attempt to maintain the function pointer and component pointer if someone changes the target object and it has the correct component type on it.
                if (oldTargetObject != null && newTargetObject != null)
                {
                    if (oldTargetObject.GetType() != newTargetObject.GetType()) // If not an asset, if it is an asset and the same type we don't do anything
                    {
                        // If these are Unity components then the game object that they are attached to may have multiple copies of the same component type so attempt to match the count
                        if (typeof(Component).IsAssignableFrom(oldTargetObject.GetType()) && newTargetObject.GetType() == typeof(GameObject))
                        {
                            GameObject oldParentObject = ((Component)oldTargetObject).gameObject;
                            GameObject newParentObject = (GameObject)newTargetObject;

                            Component[] oldComponentList = oldParentObject.GetComponents(oldTargetObject.GetType());

                            int componentLocationOffset = 0;
                            for (int i = 0; i < oldComponentList.Length; ++i)
                            {
                                if (oldComponentList[i] == oldTargetObject)
                                    break;

                                if (oldComponentList[i].GetType() == oldTargetObject.GetType()) // Only take exact matches for component type since I don't want to do redo the reflection to find the methods at the moment.
                                    componentLocationOffset++;
                            }

                            Component[] newComponentList = newParentObject.GetComponents(oldTargetObject.GetType());

                            int newComponentIndex = 0;
                            int componentCount = -1;
                            for (int i = 0; i < newComponentList.Length; ++i)
                            {
                                if (componentCount == componentLocationOffset)
                                    break;

                                if (newComponentList[i].GetType() == oldTargetObject.GetType())
                                {
                                    newComponentIndex = i;
                                    componentCount++;
                                }
                            }

                            if (newComponentList.Length > 0 && newComponentList[newComponentIndex].GetType() == oldTargetObject.GetType())
                            {
                                serializedTarget.objectReferenceValue = newComponentList[newComponentIndex];
                            }
                            else
                            {
                                serializedMethod.stringValue = null;
                            }
                        }
                        else
                        {
                            serializedMethod.stringValue = null;
                        }
                    }
                }
                else
                {
                    serializedMethod.stringValue = null;
                }
            }

            PersistentListenerMode mode = (PersistentListenerMode)serializedMode.intValue;

            SerializedProperty argument;
            if (serializedTarget.objectReferenceValue == null || string.IsNullOrEmpty(serializedMethod.stringValue))
                mode = PersistentListenerMode.Void;

            switch (mode)
            {
                case PersistentListenerMode.Object:
                case PersistentListenerMode.String:
                case PersistentListenerMode.Bool:
                case PersistentListenerMode.Float:
                    argument = serializedArgs.FindPropertyRelative("m_" + System.Enum.GetName(typeof(PersistentListenerMode), mode) + "Argument");
                    break;
                default:
                    argument = serializedArgs.FindPropertyRelative("m_IntArgument");
                    break;
            }

            string argTypeName = serializedArgs.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
            System.Type argType = typeof(Object);
            if (!string.IsNullOrEmpty(argTypeName))
                argType = FindTypeInAllAssemblies(argTypeName) ?? typeof(Object);

            if (mode == PersistentListenerMode.Object)
            {
                EditorGUI.BeginChangeCheck();
                Object result = EditorGUI.ObjectField(argRect, GUIContent.none, argument.objectReferenceValue, argType, true);
                if (EditorGUI.EndChangeCheck())
                    argument.objectReferenceValue = result;
            }
            else if (mode != PersistentListenerMode.Void && mode != PersistentListenerMode.EventDefined)
                EditorGUI.PropertyField(argRect, argument, GUIContent.none);

            EditorGUI.BeginDisabledGroup(serializedTarget.objectReferenceValue == null);
            {
                EditorGUI.BeginProperty(functionRect, GUIContent.none, serializedMethod);

                GUIContent buttonContent;

                if (EditorGUI.showMixedValue)
                {
                    buttonContent = new GUIContent("\u2014", "Mixed Values");
                }
                else
                {
                    if (serializedTarget.objectReferenceValue == null || string.IsNullOrEmpty(serializedMethod.stringValue))
                    {
                        buttonContent = new GUIContent("No Function");
                    }
                    else
                    {
                        buttonContent = new GUIContent(GetFunctionDisplayName(serializedTarget, serializedMethod, mode, argType, Settings.displayArgumentType));
                    }
                }

                if (GUI.Button(functionRect, buttonContent, EditorStyles.popup))
                {
                    BuildPopupMenu(serializedTarget.objectReferenceValue, element, argType).DropDown(functionRect);
                }

                EditorGUI.EndProperty();
            }
            EditorGUI.EndDisabledGroup();
        }

        void SelectCallback(ReorderableList list)
        {
            currentState.lastSelectedIndex = list.index;
        }

        void ReorderCallback(ReorderableList list)
        {
            currentState.lastSelectedIndex = list.index;
        }

        void AddEventListener(ReorderableList list)
        {
            if (listenerArray.hasMultipleDifferentValues)
            {
                foreach (Object targetObj in listenerArray.serializedObject.targetObjects)
                {
                    SerializedObject tempSerializedObject = new SerializedObject(targetObj);
                    SerializedProperty listenerArrayProperty = tempSerializedObject.FindProperty(listenerArray.propertyPath);
                    listenerArrayProperty.arraySize += 1;
                    tempSerializedObject.ApplyModifiedProperties();
                }

                listenerArray.serializedObject.SetIsDifferentCacheDirty();
                listenerArray.serializedObject.Update();
                list.index = list.serializedProperty.arraySize - 1;
            }
            else
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
            }

            currentState.lastSelectedIndex = list.index;

            // Init default state
            SerializedProperty serialiedListener = listenerArray.GetArrayElementAtIndex(list.index);
            ResetEventState(serialiedListener);
        }

        void RemoveCallback(ReorderableList list)
        {
            if (currentState.reorderableList.count > 0)
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                currentState.lastSelectedIndex = list.index;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            currentLabelText = label.text;
            PrepareState(property);

            if (dummyEvent == null)
                return;

            if (currentState.reorderableList != null)
            {
                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                currentState.reorderableList.DoList(position);
                EditorGUI.indentLevel = oldIndent;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            PrepareState(property);

            float height = 0f;
            if (currentState.reorderableList != null)
                height = currentState.reorderableList.GetHeight();

            return height;
        }
    }
}