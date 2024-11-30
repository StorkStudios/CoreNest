using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GUIStyleStateExtensions
{
    public static GUIStyleState CreateCopy(this GUIStyleState self)
    {
        return new GUIStyleState
        {
            textColor = self.textColor,
            scaledBackgrounds = (Texture2D[])self.scaledBackgrounds.Clone(),
            background = self.background
        };
    }
}
