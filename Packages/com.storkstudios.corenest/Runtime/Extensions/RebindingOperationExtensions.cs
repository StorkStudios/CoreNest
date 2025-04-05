using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

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
