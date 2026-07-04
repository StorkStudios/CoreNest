using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    public class ColliderEvents : MonoBehaviour
    {
        [SerializeField]
        [FoldoutGroup("Collision 3D")]
        private UnityEvent<Collision> collisionEntered = new UnityEvent<Collision>();
        [SerializeField]
        [FoldoutGroup("Collision 3D")]
        private UnityEvent<Collision> collisionStayed = new UnityEvent<Collision>();
        [SerializeField]
        [FoldoutGroup("Collision 3D")]
        private UnityEvent<Collision> collisionExited = new UnityEvent<Collision>();
        [SerializeField]
        [FoldoutGroup("Collision 2D")]
        private UnityEvent<Collision2D> collisionEntered2D = new UnityEvent<Collision2D>();
        [SerializeField]
        [FoldoutGroup("Collision 2D")]
        private UnityEvent<Collision2D> collisionStayed2D = new UnityEvent<Collision2D>();
        [SerializeField]
        [FoldoutGroup("Collision 2D")]
        private UnityEvent<Collision2D> collisionExited2D = new UnityEvent<Collision2D>();
        [SerializeField]
        [FoldoutGroup("Trigger 3D")]
        private UnityEvent<Collider> triggerEntered = new UnityEvent<Collider>();
        [SerializeField]
        [FoldoutGroup("Trigger 3D")]
        private UnityEvent<Collider> triggerStayed = new UnityEvent<Collider>();
        [SerializeField]
        [FoldoutGroup("Trigger 3D")]
        private UnityEvent<Collider> triggerExited = new UnityEvent<Collider>();
        [SerializeField]
        [FoldoutGroup("Trigger 2D")]
        private UnityEvent<Collider2D> triggerEntered2D = new UnityEvent<Collider2D>();
        [SerializeField]
        [FoldoutGroup("Trigger 2D")]
        private UnityEvent<Collider2D> triggerStayed2D = new UnityEvent<Collider2D>();
        [SerializeField]
        [FoldoutGroup("Trigger 2D")]
        private UnityEvent<Collider2D> triggerExited2D = new UnityEvent<Collider2D>();

        public event UnityAction<Collision> CollisionEntered
        {
            add => collisionEntered.AddListener(value);
            remove => collisionEntered.RemoveListener(value);
        }
        public event UnityAction<Collision> CollisionStayed
        {
            add => collisionStayed.AddListener(value);
            remove => collisionStayed.RemoveListener(value);
        }
        public event UnityAction<Collision> CollisionExited
        {
            add => collisionExited.AddListener(value);
            remove => collisionExited.RemoveListener(value);
        }
        public event UnityAction<Collision2D> CollisionEntered2D
        {
            add => collisionEntered2D.AddListener(value);
            remove => collisionEntered2D.RemoveListener(value);
        }
        public event UnityAction<Collision2D> CollisionStayed2D
        {
            add => collisionStayed2D.AddListener(value);
            remove => collisionStayed2D.RemoveListener(value);
        }
        public event UnityAction<Collision2D> CollisionExited2D
        {
            add => collisionExited2D.AddListener(value);
            remove => collisionExited2D.RemoveListener(value);
        }
        public event UnityAction<Collider> TriggerEntered
        {
            add => triggerEntered.AddListener(value);
            remove => triggerEntered.RemoveListener(value);
        }
        public event UnityAction<Collider> TriggerStayed
        {
            add => triggerStayed.AddListener(value);
            remove => triggerStayed.RemoveListener(value);
        }
        public event UnityAction<Collider> TriggerExited
        {
            add => triggerExited.AddListener(value);
            remove => triggerExited.RemoveListener(value);
        }
        public event UnityAction<Collider2D> TriggerEntered2D
        {
            add => triggerEntered2D.AddListener(value);
            remove => triggerEntered2D.RemoveListener(value);
        }
        public event UnityAction<Collider2D> TriggerStayed2D
        {
            add => triggerStayed2D.AddListener(value);
            remove => triggerStayed2D.RemoveListener(value);
        }
        public event UnityAction<Collider2D> TriggerExited2D
        {
            add => triggerExited2D.AddListener(value);
            remove => triggerExited2D.RemoveListener(value);
        }

        private void OnCollisionEnter(Collision collision)
        {
            collisionEntered.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            collisionStayed.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            collisionExited.Invoke(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            collisionEntered2D.Invoke(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            collisionStayed2D.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            collisionExited2D.Invoke(collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            triggerEntered.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            triggerStayed.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            triggerExited.Invoke(other);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            triggerEntered2D.Invoke(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            triggerStayed2D.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            triggerExited2D.Invoke(collision);
        }
    }
}
