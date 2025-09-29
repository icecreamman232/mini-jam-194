using SGGames.Script.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private ButtonController m_playButton;
    [SerializeField] private RectTransform m_title1;
    [SerializeField] private RectTransform m_title2;
    [SerializeField] private RectTransform m_title3;

    private void Awake()
    {
        m_playButton.OnButtonClick = PressPlay;
        Show();
    }

    private void Show()
    {
        m_canvasGroup.Activate();
    }
    
    private void Hide()
    {
        m_canvasGroup.Deactivate();
    }

    private void PressPlay()
    {
        Hide();
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
