using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class GUIStyleExtensions
    {
        public static void CopyInto(this GUIStyle source, GUIStyle target)
        {
            target.active = source.active.CreateCopy();
            target.alignment = source.alignment;
            target.border = source.border.CreateCopy();
            target.clipping = source.clipping;
            target.contentOffset = source.contentOffset;
            target.fixedHeight = source.fixedHeight;
            target.fixedWidth = source.fixedWidth;
            target.focused = source.focused.CreateCopy();
            target.font = source.font;
            target.fontSize = source.fontSize;
            target.fontStyle = source.fontStyle;
            target.hover = source.hover.CreateCopy();
            target.imagePosition = source.imagePosition;
            target.margin = source.margin;
            target.name = source.name;
            target.normal = source.normal.CreateCopy();
            target.onActive = source.onActive.CreateCopy();
            target.onFocused = source.onFocused.CreateCopy();
            target.onHover = source.onHover.CreateCopy();
            target.onNormal = source.onNormal.CreateCopy();
            target.overflow = source.overflow.CreateCopy();
            target.padding = source.padding.CreateCopy();
            target.richText = source.richText;
            target.stretchHeight = source.stretchHeight;
            target.stretchWidth = source.stretchWidth;
            target.wordWrap = source.wordWrap;
        }
    }
}