using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace StorkStudios.CoreNest
{
    public static class UIUtils
    {
        public enum Fade { FadeIn, FadeOut }

        public static IEnumerator AnimateLocalScaleCoroutine(Transform transform, float duration, Vector3 targetScale, Func<bool> additionalConditions = null)
        {
            additionalConditions ??= () => true;

            float counter = 0;
            Vector3 beginningScale = transform.localScale;

            while (counter < duration && additionalConditions())
            {
                counter += Time.unscaledDeltaTime;

                transform.localScale = Vector3.Lerp(beginningScale, targetScale, counter / duration);

                yield return null;
            }

            if (additionalConditions())
            {
                transform.localScale = targetScale;
            }
        }

        public static IEnumerator TextFadeCoroutine(TextMeshProUGUI text, float duration, Fade fadeDirection, Func<bool> additionalConditions = null, bool disableGameObject = true)
        {
            additionalConditions ??= () => true;

            float counter = 0;
            float start = fadeDirection == Fade.FadeIn ? 0 : 1;
            float end = fadeDirection == Fade.FadeIn ? 1 : 0;

            if (!text.gameObject.activeSelf)
            {
                text.gameObject.SetActive(true);
            }

            Color vertexColor = text.color;

            while (counter < duration && additionalConditions())
            {
                counter += Time.unscaledDeltaTime;

                float alpha = Mathf.Lerp(start, end, counter / duration);

                text.color = new Color(vertexColor.r, vertexColor.g, vertexColor.b, alpha);

                yield return null;
            }

            if (additionalConditions())
            {
                text.color = new Color(vertexColor.r, vertexColor.g, vertexColor.b, end);

                if (disableGameObject && fadeDirection == Fade.FadeOut)
                {
                    text.gameObject.SetActive(false);
                }
            }
        }

        public static IEnumerator CanvasGroupFadeCoroutine(CanvasGroup canvasGroup, float duration, Fade fadeDirection, Func<bool> additionalConditions = null, bool disableGameObject = true, bool useCurrentAlpha = false)
        {
            additionalConditions ??= () => true;

            float counter = 0;
            float start = useCurrentAlpha ? canvasGroup.alpha : (fadeDirection == Fade.FadeIn ? 0 : 1);
            float end = fadeDirection == Fade.FadeIn ? 1 : 0;

            if (!canvasGroup.gameObject.activeSelf)
            {
                canvasGroup.gameObject.SetActive(true);
            }

            while (counter < duration && additionalConditions())
            {
                counter += Time.unscaledDeltaTime;

                float alpha = Mathf.Lerp(start, end, counter / duration);

                canvasGroup.alpha = alpha;

                yield return null;
            }

            if (additionalConditions())
            {
                canvasGroup.alpha = end;
                if (disableGameObject && fadeDirection == Fade.FadeOut)
                {
                    canvasGroup.gameObject.SetActive(false);
                }
            }
        }

        public static IEnumerator AnimatePositionCoroutine(Transform transform, float duration, Vector3 position, Func<bool> additionalConditions = null)
        {
            additionalConditions ??= () => true;

            float counter = 0;
            Vector3 startingPosition = transform.position;

            while (counter < duration && additionalConditions())
            {
                counter += Time.unscaledDeltaTime;

                transform.position = Vector3.Lerp(startingPosition, position, counter / duration);

                yield return null;
            }

            if (additionalConditions())
            {
                transform.position = position;
            }
        }

        public static IEnumerator AnimateRectTransformCoroutine(RectTransform rectTransform, float duration, Vector2 position, Vector2 sizeDelta, Func<bool> additionalConditions = null)
        {
            additionalConditions ??= () => true;

            float counter = 0;
            Vector2 startingPosition = rectTransform.position;
            Vector2 startingSize = rectTransform.sizeDelta;

            while (counter < duration && additionalConditions())
            {
                counter += Time.unscaledDeltaTime;

                Vector2 p = Vector2.Lerp(startingPosition, position, counter / duration);
                Vector2 s = Vector2.Lerp(startingSize, sizeDelta, counter / duration);

                rectTransform.sizeDelta = s;
                rectTransform.position = p;

                yield return null;
            }

            if (additionalConditions())
            {
                rectTransform.position = position;
                rectTransform.sizeDelta = sizeDelta;
            }
        }

        public static IEnumerator AnimateRectTransformAnchoredPositionCoroutine(RectTransform rectTransform, float duration, Vector2 anchoredPosition, Vector2 sizeDelta, Func<bool> additionalConditions = null)
        {
            additionalConditions ??= () => true;

            float counter = 0;
            Vector2 startingPosition = rectTransform.anchoredPosition;
            Vector2 startingSize = rectTransform.sizeDelta;

            while (counter < duration && additionalConditions())
            {
                counter += Time.unscaledDeltaTime;

                Vector2 p = Vector2.Lerp(startingPosition, anchoredPosition, counter / duration);
                Vector2 s = Vector2.Lerp(startingSize, sizeDelta, counter / duration);

                rectTransform.sizeDelta = s;
                rectTransform.anchoredPosition = p;

                yield return null;
            }

            if (additionalConditions())
            {
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }
        }

        public static Vector2 CalculateAnchoredPositionRelativeTo(RectTransform rectPosition, RectTransform newSpace)
        {
            float originalX = rectPosition.rect.width * rectPosition.pivot.x + rectPosition.rect.xMin;
            float originalY = rectPosition.rect.height * rectPosition.pivot.y + rectPosition.rect.yMin;
            Vector2 fromPivotDerivedOffset = new Vector2(originalX, originalY);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, rectPosition.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(newSpace, screenP, null, out Vector2 localPoint);
            float newX = newSpace.rect.width * newSpace.pivot.x + newSpace.rect.xMin;
            float newY = newSpace.rect.height * newSpace.pivot.y + newSpace.rect.yMin;
            Vector2 pivotDerivedOffset = new Vector2(newX, newY);
            return newSpace.anchoredPosition + localPoint - pivotDerivedOffset;
        }

        public static Vector2 CalculateSizeDeltaRelativeTo(RectTransform rectSize, RectTransform newSpace)
        {
            return rectSize.rect.size - newSpace.rect.size + newSpace.sizeDelta;
        }
    }
}