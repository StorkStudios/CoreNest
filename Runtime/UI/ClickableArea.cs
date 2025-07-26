using UnityEngine;
using UnityEngine.UI;

namespace StorkStudios.CoreNest
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ClickableArea : Graphic
    {
        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            return true;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}