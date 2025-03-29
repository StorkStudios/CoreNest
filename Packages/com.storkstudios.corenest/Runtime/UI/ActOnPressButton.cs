using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActOnPressButton : Button, IPointerDownHandler, IPointerClickHandler
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        //do nothing
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        base.OnPointerClick(eventData);
    }
}
