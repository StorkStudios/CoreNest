using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class RectOffsetExtensions
    {
        public static RectOffset CreateCopy(this RectOffset self)
        {
            return new RectOffset(self.left, self.right, self.top, self.bottom);
        }
    }
}