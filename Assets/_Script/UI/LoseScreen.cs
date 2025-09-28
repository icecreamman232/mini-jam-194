using System;
using SGGames.Script.Core;
using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private ButtonController m_menuButton;
    [SerializeField] private ButtonController m_restartButton;
    [SerializeField] private GameEvent m_gameEvent;

    private void Awake()
    {
        m_canvasGroup.Deactivate();
        m_gameEvent.AddListener(OnReceiveGameEvent);
        m_restartButton.OnButtonClick = OnRestartButtonClicked;
    }
    
    private void OnDestroy()
    {
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
        m_restartButton.OnButtonClick = null;
    }

    private void OnRestartButtonClicked()
    {
        m_gameEvent.Raise(GameEventType.RestartGame);
        m_canvasGroup.Deactivate();
    }

    private void OnReceiveGameEvent(GameEventType gameEventType)
    {
        if (gameEventType == GameEventType.GameOver)
        {
            m_canvasGroup.Activate();
        }
    }
}
