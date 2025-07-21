using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component used for creating UI bars that use the <see cref="Image.fillAmount"/> option of the <see cref="Image"/> component.
/// </summary>
public class UIBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private List<Image> images = new List<Image>();

    [Header("Config")]
    [SerializeField]
    private bool animateColor;
    [SerializeField]
    private Gradient gradient;

    [Header("Debug")]
    [SerializeField]
    [Range(0, 1)]
    private float value;

    private void OnValidate()
    {
        ChangeValue(value, 1);
    }

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
        float t = Mathf.Clamp01(currentValue / maxValue);
        foreach (Image image in images.Where(image => image != null))
        {
            image.fillAmount = t;

            if (animateColor)
            {
                image.color = gradient.Evaluate(t);
            }
        }
    }

    /// <summary>
    /// Sets the <see cref="Behaviour.enabled"/> value of the image at the specified index in the <see cref="images"/> list.
    /// </summary>
    public void ImageSetEnabled(int index, bool enabled)
    {
        if (index > images.Count || index < 0)
        {
            return;
        }
        images[index].enabled = enabled;
    }
}
