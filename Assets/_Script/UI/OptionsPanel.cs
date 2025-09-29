using SGGames.Script.Core;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;
    [SerializeField] private ButtonController m_backButton;

    private SoundManager m_soundManager;
    
    private void Start()
    {
        m_canvasGroup.Deactivate();
        m_musicSlider.value = 1;
        m_sfxSlider.value = 1;
        m_musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        m_sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        m_soundManager = ServiceLocator.GetService<SoundManager>();
    }

    private void OnDestroy()
    {
        m_musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        m_sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
    }

    private void OnSfxVolumeChanged(float currentVolume)
    {
        m_soundManager.ChangeSfxVolume(currentVolume);
    }

    private void OnMusicVolumeChanged(float currentVolume)
    {
        m_soundManager.ChangeMusicVolume(currentVolume);
    }

    public void Show()
    {
        m_canvasGroup.Activate();
    }

    public void Hide()
    {
        m_canvasGroup.Deactivate();
    }
}
