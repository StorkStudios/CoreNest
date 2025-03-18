using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SampleComponent : MonoBehaviour
{
    public enum TestEnum { Value1,  Value2 };

    public SerializedDictionary<string, int> testDictionary;
    public SerializedDictionary<TestEnum, string> testEnumDictionary;

    public Tag tag1;
    public Tag tag2;

    [EditObjectInInspector]
    public MovingEnvironmentElement sc;

    [InvokeButton]
    public void PrintTags()
    {
        print($"{tag1.GetTagString()} and {tag2.GetTagString()}");
    }
}
