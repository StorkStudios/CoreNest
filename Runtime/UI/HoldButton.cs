using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : Button, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent<float> buttonDownUpdate;
    public UnityEvent buttonDown;

    public bool IsHeld { get; private set; } = false;
    private float pressTimestamp;

    private void Update()
    {
        if (!IsHeld && IsPressed())
        {
            IsHeld = true;
            pressTimestamp = Time.time;
            buttonDown.Invoke();
        }
        if (IsHeld)
        {
            if (!IsPressed())
            {
                IsHeld = false;
                onClick.Invoke();
            }
            else
            {
                buttonDownUpdate.Invoke(Time.time - pressTimestamp);
            }
        }
    }
}