using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleComponent : MonoBehaviour
{
    public enum TestEnum { Value1,  Value2 };

    public SerializedDictionary<string, int> testDictionary;
    public SerializedDictionary<TestEnum, string> testEnumDictionary;

    public Tag tag1;
    public Tag tag2;

    public bool loadSceneOnStart = false;
    public Scene scene;
    
    [EditObjectInInspector]
    public MovingEnvironmentElement sc;

    private void Start()
    {
        if (loadSceneOnStart)
        {
            this.CallDelayed(3, () => SceneManager.LoadScene(scene.GetBuildIndex()));
        }
        this.CallDelayed(3, () => print(SamplerComponenter.Instance.b));
    }

    [InvokeButton]
    public void PrintBulech()
    {
        Print("Bulech");
    }

    [InvokeButton]
    public void Print(string str)
    {
        print(str);
    }
}
