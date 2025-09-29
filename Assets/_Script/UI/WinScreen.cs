using SGGames.Script.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        m_menuButton.OnButtonClick = OnMenuButtonClicked;
        m_restartButton.OnButtonClick = OnRestartButtonClicked;
    }

    private void OnRestartButtonClicked()
    {
        m_gameEvent.Raise(GameEventType.RestartGame);
        m_canvasGroup.Deactivate();
    }

    private void OnMenuButtonClicked()
    {
        ServiceLocator.ClearServices();
        SceneManager.LoadScene("Menu");
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
