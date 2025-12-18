# Core'Nest - Stork'Studios' core unity scripts

This package contains runtime and editor logic used in games by Stork'Studios. It supports common development patterns across Unity projects such as:
- Editor tooling
- Rich inspector attributes
- Serialized data structures
- Singletons
- UI components

## Installation

1. Open Package Manager window
1. Click `+/Install package from git URL...`
1. Enter `https://github.com/StorkStudios/CoreNest.git`
1. Click `Install`

## What's inside?

<ul>
<li><details><summary>Runtime</summary><ul>
    <li><details><summary>Attributes</summary><ul>
        <li><details><summary>StringValueAttribute</summary>Used for providing a string value of enumeration members</detials></li>
    </ul></details></li>
    <li><details><summary>Collections</summary><ul> 
        <li><details><summary>EventMap</summary>A dictionary of <code>TKey -> event</code></detials></li>
        <li><details><summary>NullableObject</summary>A hashable version of <code>type?</code> that can be used to make nullable types dictionary keys</detials></li>
        <li><details><summary>SerializationArrayWrapper</summary>A wrapper that can be used to create nested lists: <code>List&lt;SerializationArrayWrapper&lt;T>></code></detials></li>
        <li><details><summary>SerializedDictionary</summary>A dictionary serializable by Unity</detials></li>
        <li><details><summary>SerializedLinkedList</summary>A linked list serializable by Unity</detials></li>
        <li><details><summary>SerializedPair</summary>A key-value pair serializable by Unity</detials></li>
        <li><details><summary>SerializedQueue</summary>A queue serializable by Unity</detials></li>
        <li><details><summary>SerializedSet</summary>A hashset serializable by Unity</detials></li>
    </ul></details></li>
    <li><details><summary>Components</summary><ul>
        <li><details><summary>UI</summary><ul>
            <li><details><summary>ActOnPressButton</summary><code>UnityEngine.UI.Button</code> that gets clicked on pointer down (normally it's on pointer up)</detials></li>
            <li><details><summary>ClickableArea</summary>Makes this rect transform clickable without having a texture (because transparent Images are ignored during raycast)</detials></li>
            <li><details><summary>HoldButton</summary><code>UnityEngine.UI.Button</code> that also measures time between pointer down and up</detials></li>
            <li><details><summary>UIBarController</summary>Component for creating ui bars (eg. health bar) that uses <code>Image.fillAmount</code> (instead of animating anchors like in built in Slider) </detials></li>
        </ul></detials></li>
        <li><details><summary>ActiveCoroutineContext</summary>A persistent context used for coroutines (PersistentSingleton)</detials></li>
        <li><details><summary>Animation Event Converter</summary>Makes managing animation events easier by having them call function name <code>ExternalName</code> with a string argument specifying what UnityEvent to call</detials></li>
        <li><details><summary>MovingEnvironmentElement</summary>Component allowing for lerping a target transform between two points (position with rotation)</detials></li>
        <li><details><summary>PersistentSingleton</summary>Inherit to make a script a singleton that lives in <code>DontDestroyOnLoad</code> and creates itself when referenced and not present</detials></li>
        <li><details><summary>Singleton</summary>Inherit to make a script a singleton that lives on the current scene (the object containing this must be created manually)</detials></li>
    </ul></details></li>
    <li><details><summary>Data</summary><ul>
        <li><details><summary>ActionBinding</summary>Class with a custom property drawer for getting InputSystem's action binding</detials></li>
        <li><details><summary>IndentedStringBuilder</summary>A string builder with indentation support<ul>
            <li><details><summary>IndentScope</summary>IDisposable for creating a scope with indentation</detials></li>
        </ul></detials></li>
        <li><details><summary>InterruptingCoroutine</summary>A wrapper for a coroutine that executes an action after specipied wait time which can be stopped</detials></li>
        <li><details><summary>LazyComponentReference</summary>A wrapper class for component reference that calls <code>Component.GetComponent</code> when getting the value</detials></li>
        <li><details><summary>ObservableVariable</summary>A wrapper for a variable that creates a <code>ValueChanged</code> event for observing its value</detials></li>
        <li><details><summary>PriorityEvent</summary>An event that supports assigning a call priority for subscribing callbacks. (Normally when subscribing to an event the callbacks are preformed in FIFO fashion)</detials></li>
        <li><details><summary>RangeBoundaries</summary>A class for defining min-max boundaries and checking if a value lies inside<ul>
            <li><details><summary>RangeBoundariesFloat</summary>A float type boundaries with additional functionalities (GetRandomBetween, GetAverage, NormalizeValue)</detials></li>
        </ul></detials></li>
        <li><details><summary>ScriptableObjectListWrapper</summary>A simple base class for having a single data list of specified type<ul>
            <li><details><summary>ScriptableObjectStringList</summary>A scriptable object containing a single string list</detials></li>
        </ul></detials></li>
        <li><details><summary>ScriptableObjectSingleton</summary>Inherit to make a scriptable object a singleton. The object has to be created inside <code>Assets/Resources</code> folder</detials></li>
        <li><details><summary>SpeedDistanceTimeConfig</summary>Class for defining a motion (eg. player dash). Useful when you don't know whether it should be speed in time, distance over time or distance in speed</detials></li>
        <li><details><summary>Vector2XZ</summary>A Vector2 representing a direction on XZ horizontal plane</detials></li>
    </ul></details></li>
    <li><details><summary>Extensions</summary><ul>
        <li><details><summary>AnimationCurve</summary><ul>
            <li><details><summary>EvaluateUnclamped</summary>Evaluate curve with time clamped between first and last keyframe time (wiem, że bez sensu, uświadomiłem sobie to jak teraz piszę dokumentację)</detials></li>
            <li><details><summary>GetNormalizedAnimationCurve</summary>Creates a new normalized AnimationCurve. Normalized curve is defined as having all keyframe times and values inside [0, 1] range </detials></li>
        </ul></detials></li>
        <li><details><summary>IEnumerable</summary><ul>
            <li><details><summary>&lt;Vector3>Average</summary>Gets an average (sum / count) vector</detials></li>
            <li><details><summary>GetRandomElement</summary>Gets a random element using static <code>UnityEngine.Random</code></detials></li>
            <li><details><summary>Shuffled</summary>Returns a shuffled IEnumerable using <code>UnityEngine.Random</code></detials></li>
            <li><details><summary>GetRandomElementWeighted</summary>Gets a random element using provided weight function and <code>UnityEngine.Random</code></detials></li>
        </ul></detials></li>
        <li><details><summary>LayerMask</summary><ul>
            <li><details><summary>ContainsLayer</summary>Checks whether the corresponding layer bit is set in the mask</detials></li>
            <li><details><summary>GetLayers</summary>Returns a List of layer indices that are set in the mask</detials></li>
        </ul></detials></li>
        <li><details><summary>LinkedList</summary><ul>
            <li><details><summary>RemoveSafeForEach</summary>Performs an action on each element of the linked list in a way that allows for its removal from the collection</detials></li>
            <li><details><summary>RemoveSafeForEachNode</summary>Performs an action on each node of the linked list in a way that allows for its removal from the collection</detials></li>
            <li><details><summary>AppendLinkedList</summary>Moves all nodes of the other linked list (it gets cleared) to the end of this</detials></li>
            <li><details><summary>Add</summary>Wrapper of <code>LinkedList.AddLast</code> that allows for collection initialization <code>new LinkedList&lt;int> { 1, 2, 3 }</code></detials></li>
        </ul></detials></li>
        <li><details><summary>List</summary><ul>
            <li><details><summary>ShuffleSelf</summary>Shuffles itself using static <code>UnityEngine.Random</code></detials></li>
        </ul></detials></li>
        <li><details><summary>MonoBehaviour</summary><ul>
            <li><details><summary>CallDelayed</summary>Creates a Coroutine that performs an action after set delay. To call this on this object use <code>this.CallDelayed</code></detials></li>
            <li><details><summary>CallNextFrame</summary>Creates a Coroutine that performs an action on the next frame. To call this on this object use <code>this.CallDelayed</code></detials></li>
        </ul></detials></li>
        <li><details><summary>PointerEventData</summary><ul>
            <li><details><summary>GetNextRaycastTarget</summary>Performs a pointer raycast and returns the next object that would be hit by the pointer. Useful for ovelaping controls</detials></li>
        </ul></detials></li>
        <li><details><summary>RebindingOperation</summary><ul>
            <li><details><summary>WithControlsExcludingAll</summary>Applies <code>RebindingOperation.WithControlsExcluding</code> over the provided paths</detials></li>
        </ul></detials></li>
        <li><details><summary>TMP_TextInfo</summary><ul>
            <li><details><summary>SetCharacterColor</summary>Writes a character color into the text render buffer</detials></li>
            <li><details><summary>GetCharacterColorCorners</summary>Retrieves the colors of the corners of the character quad from the text render buffer</detials></li>
        </ul></detials></li>
        <li><details><summary>Vector2</summary><ul>
            <li><details><summary>ToVector3</summary>Creates a Vector3 from this vector</detials></li>
        </ul></detials></li>
        <li><details><summary>Vector3</summary><ul>
            <li><details><summary>NormalizedWithCutoff</summary>Returns a normalized vector or zero if the magnitude is less than specified</detials></li>
            <li><details><summary>ProjectOnSimplePlane</summary>Less computation heavy (at least i think so) projection on a global axis aligned plane (XZ, XY, YZ)</detials></li>
        </ul></detials></li>
    </ul></details></li>
    <li><details><summary>Generated</summary>These types are automatically generated by the editor scripts<ul>
        <li><details><summary>Layer</summary>Enum containing all defined layers with correct indices</detials></li>
        <li><details><summary>Scene</summary>Enum containing all scenes in build's scene list with correct build indices</detials></li>
        <li><details><summary>Tag</summary>Enum containing all defined tags with <code>StringValueAttribute</code> containing the correct tag string</detials></li>
    </ul></details></li>
    <li><details><summary>Interfaces</summary><ul>
        <li><details><summary>IGenericInterface</summary>When using <code>RequireInterfaceAttribute</code> with generic interface it must inherit from this for the attribute to work propery</detials></li>
        <li><details><summary>IInvokeable</summary>Used for passing objects that can be invoked (eg. into the <code>LinkedList.RemoveSafeForEach</code> extension to not create garbage by creating lambdas)</detials></li>
    </ul></details></li>
    <li><details><summary>Utils</summary><ul>
        <li><details><summary>MathUtils</summary><ul>
            <li><details><summary>&lt;Vector3>InverseLerp</summary>Inverse lerp for Vector3</detials></li>
            <li><details><summary>Mod</summary>Modulo that returns a positive value (so the array index wrapping works for negative values)</detials></li>
        </ul></detials></li>
        <li><details><summary>UIUtils</summary>This mainly contains coroutines used for animations. All coroutines support additional "continue running" condition check<ul>
            <li><details><summary>AnimateLocalScaleCoroutine</summary>Animates a transform's local scale to target vale over time</detials></li>
            <li><details><summary>TextFadeCoroutine</summary>Fades text ([0 -> 1] or [1 -> 0]) over time</detials></li>
            <li><details><summary>CanvasGroupFadeCoroutine</summary>Fades canvas group ([0 -> 1] or [1 -> 0] or [current value -> 0/1]) over time</detials></li>
            <li><details><summary>AnimatePositionCoroutine</summary>Lerps transform's position from current to target over time</detials></li>
            <li><details><summary>AnimateRectTransformCoroutine</summary>Lerps rect transform's position and size delta from current to target over time</detials></li>
            <li><details><summary>AnimateRectTransformAnchoredPositionCoroutine</summary>Lerps rect transform's anchored position and size delta from current to target over time</detials></li>
            <li><details><summary>CalculateAnchoredPositionRelativeTo</summary>Returns a Vector2 representing what anchored position would one rect transform have in another</detials></li>
            <li><details><summary>CalculateSizeDeltaRelativeTo</summary>Returns a Vector2 representing what size delta would one rect transform have in another</detials></li>
        </ul></detials></li>
        <li><details><summary>QuaternionUtils</summary><ul>
            <li><details><summary>LookRotationUpwardPriority</summary>Look rotation that proritises alinging up vector to itself (Normally up vector is used to calculate right axis and then it is recomputed)</detials></li>
        </ul></detials></li>
    </ul></details></li>
</ul></details></li>
<li><details><summary>Editor</summary><ul>
    <li><details><summary>Base Classes</summary>Classes to inherit from to create own editor functionalities<ul>
        <li><details><summary>BultInEditorExtensionBase</summary>Editor that extends a specified built in editor (useful when you want to add a button to rect transform's editor). The type name has to match <code>"Namespace.Class.Name, Assembly.Name"</code> eg. <code>"UnityEditor.RectTransformEditor, UnityEditor"</code>. To find them use <a href="https://github.com/Unity-Technologies/UnityCsReference">Unity C# reference source code on GitHub</a></detials></li>
        <li><details><summary>ScriptGeneratorBase</summary>Base class for script generators that handles content change check and file management</detials></li>
        <li><details><summary>StatefulPropertyDrawer</summary>Base class for property drawers that need to manage their state (Unity creates one property drawer for multiple properties so the state can't be stored normally inside the class)</detials></li>
    </ul></details></li>
    <li><details><summary>Disposables</summary>Used to create scopes of something<ul>
        <li><details><summary>CacheEditorStypeDisposable</summary>Caches provided editor style to be restored at the end of the scope</detials></li>
        <li><details><summary>GUIColorDisposable</summary>Caches <code>GUI.color</code> to be restored at the end of the scope</detials></li>
    </ul></details></li>
    <li><details><summary>Extensions</summary><ul>
        <li><details><summary>SerializedProperty</summary><ul>
            <li><details><summary>GetFieldInfo</summary>Get the reflection field info for the field represented by this serialized property or null if it can't be found</detials></li>
        </ul></detials></li>
    </ul></details></li>
    <li><details><summary>Rich Inspector Attributes</summary>This package contains a customized default <code>UnityEngine.Object</code> editor that allows for more handy inspector attributes<ul>
        <li><details><summary>EditObjectInInspectorAttribute</summary>Creates a foldout for <code>UnityEngine.Object</code> containing its editor</detials></li>
        <li><details><summary>FoldoutGroup</summary>Creates a foldout for all properties or invoke buttons with the same foldout id. The foldout is drawn in place of the first property or invoke button with this attribute and id</detials></li>
        <li><details><summary>InvokeButton</summary>Creates an inspector button for this method. Currently only parameterless methods are supported.</detials></li>
        <li><details><summary>MinMaxRangeAttribute</summary>Creates MinMaxSlider for this property. Only works with <code>RangeBoundariesFloat</code> and <code>Vector2</code></detials></li>
        <li><details><summary>NotNullAttribute</summary>Draws an error icon when object reference value of this property is null</detials></li>
        <li><details><summary>ReadOnlyAttribute</summary>Disables editing of this property in inspector</detials></li>
        <li><details><summary>RequireInterfaceAttribute</summary>Add checks and searching for the objects that implement provided interface. When using with generic interfaces they must inherit from <code>IGenericInterface</code></detials></li>
        <li><details><summary>ShowIfAttribute</summary>Only draws this property or invoke button if provided boolean member (field, property or parameterless method) evaluates to true</detials></li>
    </ul></details></li>
    <li><details><summary>Windows</summary><ul>
        <li><details><summary>FindScriptWindow</summary>A window for finding scripts and missing scripts in scenes and prefabs in project</detials></li>
    </ul></details></li>
    <li><details><summary>Other</summary><ul>
        <li><details><summary>FindScriptWindow</summary>A window for finding scripts and missing scripts in scenes and prefabs in project</detials></li>
    </ul></details></li>
</ul></details></li>
</ul>

Any other class that isn't described here is for implementation purposes and isn't designed for public use.