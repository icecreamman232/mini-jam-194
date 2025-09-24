using SGGames.Script.Core;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private PlayerReloadEvent m_playerReloadEvent;
    [SerializeField] private Slider m_reloadBar;

    private void Start()
    {
        m_playerReloadEvent.AddListener(UpdateReloadingBar);
        m_canvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        m_playerReloadEvent.AddListener(UpdateReloadingBar);
    }

    private void UpdateReloadingBar(ReloadEventData eventData)
    {
        if (eventData.CurrentReloadTime >= 0)
        {
            m_canvasGroup.alpha = 1;
        }
        m_reloadBar.value = MathHelpers.Remap(eventData.CurrentReloadTime, 0, eventData.MaxReloadTime, 0, 1);
        if (m_reloadBar.value >= 1)
        {
            m_canvasGroup.alpha = 0;
        }
    }
}
