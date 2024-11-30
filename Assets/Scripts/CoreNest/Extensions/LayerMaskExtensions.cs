using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool ContainsLayer(this LayerMask mask, int layer)
    {
        return ((1 << layer) & mask.value) > 0;
    }

    public static List<int> GetLayers(this LayerMask mask)
    {
        List<int> layers = new List<int>();
        int layer = 0;
        int maskValue = mask.value;
        while (maskValue > 0)
        {
            maskValue = maskValue >> 1;
            layer++;
            if ((maskValue & 1) > 0)
            {
                layers.Add(layer);
            }
        }
        return layers;
    }
}
