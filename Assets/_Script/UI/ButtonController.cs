using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerClickHandler
{
    public Action OnButtonClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick?.Invoke();
    }
}
