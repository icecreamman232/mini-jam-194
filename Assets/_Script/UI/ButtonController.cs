using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : Selectable, IPointerClickHandler
{
    public Action OnButtonClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick?.Invoke();
    }
}
