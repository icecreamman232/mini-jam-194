using DG.Tweening;
using SGGames.Script.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private ButtonController m_playButton;
    [SerializeField] private TextMeshProUGUI m_title1;
    [SerializeField] private TextMeshProUGUI m_title2;
    [SerializeField] private TextMeshProUGUI m_title3;
    [SerializeField] private TextMeshProUGUI m_authorText;

    private void Awake()
    {
        m_playButton.OnButtonClick = PressPlay;
        m_playButton.gameObject.SetActive(false);
        m_authorText.color = new Color(1, 1, 1, 0);
        Show();
    }

    private void Show()
    {
        m_canvasGroup.Activate();
        m_title1.gameObject.SetActive(true);
        m_title1.DOFontSize(m_title1.fontSize * 1.5f, 0.3f)
            .SetUpdate(isIndependentUpdate:true)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutExpo)
            .OnComplete(() =>
            {
                m_title2.gameObject.SetActive(true);
                m_title2.DOFontSize(m_title2.fontSize * 1.2f, 0.15f)
                    .SetUpdate(isIndependentUpdate:true)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InOutExpo)
                    .OnComplete(() =>
                    {
                        m_title3.gameObject.SetActive(true);
                        m_title3.DOFontSize(m_title2.fontSize * 1.8f, 0.4f)
                            .SetUpdate(isIndependentUpdate: true)
                            .SetLoops(2, LoopType.Yoyo)
                            .SetEase(Ease.InOutExpo)
                            .OnComplete(() =>
                            {
                                m_playButton.gameObject.SetActive(true);
                                m_authorText.DOFade(1, 0.3f);
                            });
                    });
            });
            
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
