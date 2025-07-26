using UnityEngine;
using UnityEngine.InputSystem;

namespace StorkStudios.CoreNest
{
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
}