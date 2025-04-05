using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ActionBinding
{
    [SerializeField]
    private InputActionReference action;
    [SerializeField]
    private string bindingId;

    public InputActionReference Action => action;
    public string BindingId => bindingId;
}
