using UnityEngine;
using UnityEngine.EventSystems;

public class FireTouchArea : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Frog.Instance.OnFireAreaTouch(eventData);
    }
}
