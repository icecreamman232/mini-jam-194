using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : Selectable, IPointerClickHandler
{
    [Header("Text")]
    [SerializeField] private Color m_normalTextColor;
    [SerializeField] private Color m_hoverTextColor;
    [SerializeField] private TextMeshProUGUI m_text;
    
    [Header("Image")]
    [SerializeField] private Image m_image;
    [SerializeField] private Color m_normalColor;
    [SerializeField] private Color m_hoverColor;
    
    public Action OnButtonClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick?.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        m_text.color = m_hoverTextColor;
        m_image.color = m_hoverColor;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        m_text.color = m_normalTextColor;
        m_image.color = m_normalColor;
        base.OnPointerExit(eventData);
    }
}
