using System.Collections.Generic;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

namespace StorkStudios.CoreNest
{
    public static class RebindingOperationExtensions
    {
        public static RebindingOperation WithControlsExcludingAll(this RebindingOperation operation, IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                operation.WithControlsExcluding(path);
            }
            return operation;
        }
    }
}