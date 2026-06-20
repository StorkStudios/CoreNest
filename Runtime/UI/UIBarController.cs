using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Component used for creating UI bars that use the <see cref="Image.fillAmount"/> option of the <see cref="Image"/> component.
    /// </summary>
    [ExecuteAlways]
    public class UIBarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private List<Image> images = new List<Image>();

        [Header("Config")]
        public bool animateColor;
        [SerializeField]
        [ShowIf(nameof(animateColor))]
        private Gradient gradient = new Gradient();
        [SerializeField]
        [ShowIf(nameof(animateColor))]
        private Color fullFillColor;
        [SerializeField]
        [ShowIf(nameof(animateColor))]
        private float fullFillAnimationDuration;

        public List<Image> Images => images;
        public Gradient Gradient => gradient;

        private float fullFillFade = 0;
        private float fillAmount = 0; // todo - this should probably be an ObserableVariable<float> or something like that

        /// <summary>
        /// Updates images with progress specified by the <paramref name="currentValue"/> and possible <paramref name="maxValue"/>.
        /// <br/>
        /// Value [0 - <paramref name="maxValue"/>] is mapped to [1 - 0] bar fill.
        /// </summary>
        public void ChangeValueInverted(float currentValue, float maxValue)
        {
            ChangeValue((maxValue - currentValue), maxValue);
        }

        /// <summary>
        /// Updates images with progress specified by the <paramref name="currentValue"/> and possible <paramref name="maxValue"/>.
        /// <br/>
        /// Value [0 - <paramref name="maxValue"/>] is mapped to [0 - 1] bar fill.
        /// </summary>
        public void ChangeValue(float currentValue, float maxValue)
        {
            fillAmount = Mathf.Clamp01(currentValue / maxValue);
        }

        private void Update()
        {
            foreach (Image image in images.Where(image => image != null))
            {
                image.fillAmount = fillAmount;

                if (animateColor)
                {
                    float delta = Time.deltaTime / fullFillAnimationDuration;
                    fullFillFade = Mathf.Clamp01(fullFillFade + (fillAmount >= 1 ? delta : -delta));
                    image.color = Color.Lerp(gradient.Evaluate(fillAmount), fullFillColor, fullFillFade);
                }
            }
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        [Range(0, 1)]
        private float debugFillAmount;

        private void OnValidate()
        {
            ChangeValue(debugFillAmount, 1);
        }
#endif
    }
}