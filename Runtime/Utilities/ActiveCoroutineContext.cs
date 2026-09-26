using UnityEngine;

namespace StorkStudios.CoreNest
{
    [Singleton(persistent: true)]
    public sealed partial class ActiveCoroutineContext : MonoBehaviour
    {
        // Coroutine context that is always active
        // Currently does nothing else
    }
}