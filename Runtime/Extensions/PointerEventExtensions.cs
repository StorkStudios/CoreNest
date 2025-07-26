using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StorkStudios.CoreNest
{
    public static class PointerEventExtensions
    {
        private static List<RaycastResult> raycastResults = new List<RaycastResult>();

        public static GameObject GetNextRaycastTarget(this PointerEventData eventData, GameObject current)
        {
            raycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            int currentIndex = raycastResults.FindIndex(e => e.gameObject == current);
            int nextObject = currentIndex + 1;
            if (nextObject >= raycastResults.Count)
            {
                return null;
            }
            return raycastResults[nextObject].gameObject;
        }
    }
}