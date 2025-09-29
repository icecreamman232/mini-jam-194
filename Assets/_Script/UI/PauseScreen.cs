using SGGames.Script.Core;
using TMPro;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private ButtonController m_resumeButton;
    [SerializeField] private ButtonController m_optionsButton;
    [SerializeField] private ButtonController m_backButton;
    [SerializeField] private OptionsPanel m_optionsPanel;
    [SerializeField] private TextMeshProUGUI m_hintText;
    [SerializeField] private GameEvent m_gameEvent;
    [SerializeField] private string[] m_hints;

    private void Start()
    {
        m_canvasGroup.Deactivate();
        m_gameEvent.AddListener(OnReceiveGameEvent);
        m_optionsButton.OnButtonClick = OnOptionsButtonClick;
        m_resumeButton.OnButtonClick = OnResumeButtonClick;
        m_backButton.OnButtonClick = OnBackFromOptions;
    }

    private void OnDestroy()
    {
        m_resumeButton.OnButtonClick = null;
        m_gameEvent.RemoveListener(OnReceiveGameEvent);
    }
    
    private void OnBackFromOptions()
    {
        m_optionsPanel.Hide();
        m_resumeButton.gameObject.SetActive(true);
        m_optionsButton.gameObject.SetActive(true);
    }
    
    private void OnOptionsButtonClick()
    {
        m_resumeButton.gameObject.SetActive(false);
        m_optionsButton.gameObject.SetActive(false);
        m_optionsPanel.Show();
    }

    private void OnResumeButtonClick()
    {
        m_gameEvent.Raise(GameEventType.UnPauseGame);
    }

    private void OnReceiveGameEvent(GameEventType eventType)
    {
        if (eventType == GameEventType.PauseGame)
        {
            m_canvasGroup.Activate();
            m_hintText.text = m_hints[Random.Range(0, m_hints.Length)];
        }
        else if (eventType == GameEventType.UnPauseGame)
        {
            m_canvasGroup.Deactivate();
        }
    }
}
