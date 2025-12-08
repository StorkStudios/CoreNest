using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    [DisallowMultipleComponent]
    public class MovingEnvironmentElement : MonoBehaviour
    {
        [System.Serializable]
        public struct Point
        {
            public Vector3 localPosition;
            public Vector3 localEuler;

            public readonly Quaternion LocalRotation => Quaternion.Euler(localEuler);

            public readonly Quaternion GetGlobalRotation(Transform parent)
            {
                return parent.rotation * LocalRotation;
            }

            public readonly Vector3 GetGlobalPosition(Transform parent)
            {
                return parent.TransformPoint(localPosition);
            }
        }

        private enum MovingElementState
        {
            MovingToB,
            MovingToA,
            A,
            B,
            None
        }

        [SerializeField]
        [NotNull]
        private Transform target;
        [SerializeField]
        private Point pointA = new Point { localPosition = new Vector3(1, 0) };
        [SerializeField]
        private Point pointB = new Point { localPosition = new Vector3(-1, 0) };
        [SerializeField]
        private float movementTime;
        [SerializeField]
        private MovingElementState defaultState;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent arrivedAtA;
        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent arrivedAtB;
        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent startedMovingToA;
        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent startedMovingToB;

        public float MovementTime => movementTime;
        public Point PointA => pointA;
        public Point PointB => pointB;

        public UnityEvent ArrivedAtA => arrivedAtA;
        public UnityEvent ArrivedAtB => arrivedAtB;
        public UnityEvent StartedMovingToA => startedMovingToA;
        public UnityEvent StartedMovingToB => startedMovingToB;

        private float time;
        private MovingElementState currentState;

        private void OnValidate()
        {
            if (target == transform || transform.GetComponentsInParent<Transform>().Contains(target))
            {
                target = null;
                Debug.LogError("Invalid moving element target. It cannot be this object or parrent of this object as it creates recursive loop when moving to target.");
            }
        }

        private void Awake()
        {
            if (target == null)
            {
                Debug.LogError($"No {nameof(target)} given to {nameof(MovingEnvironmentElement)}.");
                return;
            }

            switch (defaultState)
            {
                case MovingElementState.MovingToB:
                    ToB();
                    break;
                case MovingElementState.MovingToA:
                    ToA();
                    break;
                case MovingElementState.A:
                    ToAImmiediately();
                    break;
                case MovingElementState.B:
                    ToBImmiediately();
                    break;
            }
        }

        public void ToB()
        {
            switch (currentState)
            {
                case MovingElementState.B:
                case MovingElementState.MovingToB:
                    return;
                case MovingElementState.A:
                    time = 0;
                    break;
                case MovingElementState.MovingToA:
                    time = (1 - MathUtils.InverseLerp(pointB.GetGlobalPosition(transform), pointA.GetGlobalPosition(transform), target.position)) * movementTime;
                    break;
            }
            currentState = MovingElementState.MovingToB;
            startedMovingToB.Invoke();
        }

        public void ToA()
        {
            switch (currentState)
            {
                case MovingElementState.A:
                case MovingElementState.MovingToA:
                    return;
                case MovingElementState.B:
                    time = 0;
                    break;
                case MovingElementState.MovingToB:
                    time = (1 - MathUtils.InverseLerp(pointA.GetGlobalPosition(transform), pointB.GetGlobalPosition(transform), target.position)) * movementTime;
                    break;
            }
            currentState = MovingElementState.MovingToA;
            startedMovingToA.Invoke();
        }

        public void ChangeState()
        {
            if (currentState == MovingElementState.MovingToA || currentState == MovingElementState.A)
            {
                ToB();
            }
            else
            {
                ToA();
            }
        }

        public void ToAImmiediately()
        {
            currentState = MovingElementState.A;
            target.SetPositionAndRotation(pointA.GetGlobalPosition(transform), pointA.GetGlobalRotation(transform));
            arrivedAtA.Invoke();
        }

        public void ToBImmiediately()
        {
            currentState = MovingElementState.B;
            target.SetPositionAndRotation(pointB.GetGlobalPosition(transform), pointB.GetGlobalRotation(transform));
            arrivedAtB.Invoke();
        }

        private void Update()
        {
            if (currentState == MovingElementState.MovingToB)
            {
                time += Time.deltaTime;
                if (time >= movementTime)
                {
                    ToBImmiediately();
                }
                else
                {
                    target.SetPositionAndRotation(
                        Vector3.Lerp(pointA.GetGlobalPosition(transform), pointB.GetGlobalPosition(transform), time / movementTime),
                        Quaternion.Slerp(pointA.GetGlobalRotation(transform), pointB.GetGlobalRotation(transform), time / movementTime)
                        );
                }
            }
            if (currentState == MovingElementState.MovingToA)
            {
                time += Time.deltaTime;
                if (time >= movementTime)
                {
                    ToAImmiediately();
                }
                else
                {
                    target.SetPositionAndRotation(
                        Vector3.Lerp(pointB.GetGlobalPosition(transform), pointA.GetGlobalPosition(transform), time / movementTime),
                        Quaternion.Slerp(pointB.GetGlobalRotation(transform), pointA.GetGlobalRotation(transform), time / movementTime)
                        );
                }
            }
        }
    }
}