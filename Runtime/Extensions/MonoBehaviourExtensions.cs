using System.Collections;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine CallDelayed(this MonoBehaviour monoBehaviour, float delayTime, System.Action functionToCall)
        {
            if (delayTime <= 0)
            {
                functionToCall?.Invoke();
                return null;
            }
            return monoBehaviour.StartCoroutine(CallDelayedCoroutine(delayTime, functionToCall));
        }

        private static IEnumerator CallDelayedCoroutine(float delayTime, System.Action delayedFunction)
        {
            yield return new WaitForSeconds(delayTime);

            delayedFunction?.Invoke();
        }

        public static Coroutine CallNextFrame(this MonoBehaviour monoBehaviour, System.Action functionToCall)
        {
            return monoBehaviour.StartCoroutine(CallNextFrameCoroutine(functionToCall));
        }

        private static IEnumerator CallNextFrameCoroutine(System.Action delayedFunction)
        {
            yield return null;

            delayedFunction?.Invoke();
        }
    }
}