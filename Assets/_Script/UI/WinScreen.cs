using SGGames.Script.Core;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private ButtonController m_menuButton;
    [SerializeField] private ButtonController m_restartButton;
    [SerializeField] private GameEvent m_gameEvent;

    private void Awake()
    {
        m_canvasGroup.Deactivate();
        m_gameEvent.AddListener(OnReceiveGameEvent);
    }

    private void OnDestroy()
    {
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }

    private void OnReceiveGameEvent(GameEventType gameEventType)
    {
        if (gameEventType == GameEventType.Victory)
        {
            m_canvasGroup.Activate();
        }
    }
}
