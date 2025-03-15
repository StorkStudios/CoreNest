using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SampleComponent : MonoBehaviour
{
    public enum TestEnum { Value1,  Value2 };

    public SerializedDictionary<string, int> testDictionary;
    public SerializedDictionary<TestEnum, string> testEnumDictionary;

    [TagDropdown]
    public string tag1;
    [TagDropdown]
    public string tag2;
}
