using System;
using DG.Tweening;
using SGGames.Script.Core;
using UnityEngine;
using UnityEngine.UI;

public class ToxicBar : MonoBehaviour
{
    [SerializeField] private RectTransform m_headIcon;
    [SerializeField] private Image m_bar;
    [SerializeField] private PlayerCollectToxicEvent m_playerCollectToxicEvent;

    private void Start()
    {
        m_playerCollectToxicEvent.AddListener(OnCollectingToxic);
    }

    private void OnDestroy()
    {
        m_playerCollectToxicEvent.RemoveListener(OnCollectingToxic);
    }

    private void OnCollectingToxic(ToxicEventData eventData)
    {
        m_headIcon.DOSizeDelta(Vector2.one * 100, 0.15f)
            .SetEase(Ease.OutExpo)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                var targetFill = MathHelpers.Remap(eventData.CurrentToxic, 0, eventData.MaxToxic, 0, 1);
                m_bar.DOFillAmount(targetFill, 0.2f)
                    .SetEase(Ease.OutExpo);
            });
    }
}
