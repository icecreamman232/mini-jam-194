using SGGames.Script.Core;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_BG;
    [SerializeField] private GameEvent m_gameEvent;
    
    [SerializeField] private ButtonController m_tutorial1Button;
    [SerializeField] private ButtonController m_tutorial2Button;
    
    [SerializeField] private CanvasGroup m_tutorial1Group;
    [SerializeField] private CanvasGroup m_tutorial2Group;

    private void Awake()
    {
        m_BG.Deactivate();
        m_tutorial1Group.Deactivate();
        m_tutorial2Group.Deactivate();
        m_tutorial1Button.OnButtonClick = ShowNextTutorial;
        m_tutorial2Button.OnButtonClick = ExitTutorial;
    }

    public void ShowTutorial()
    {
        m_BG.Activate();
        Time.timeScale = 0;
        //InputManager.SetActive(false);
        m_tutorial1Group.Activate();
        m_tutorial2Group.Deactivate();
    }

    private void ShowNextTutorial()
    {
        m_tutorial1Group.Deactivate();
        m_tutorial2Group.Activate();
    }
    
    private void ExitTutorial()
    {
        m_BG.Deactivate();
        m_tutorial1Group.Deactivate();
        m_tutorial2Group.Deactivate();
        
        Time.timeScale = 1;
        InputManager.SetActive(true);
        m_gameEvent.Raise(GameEventType.UnPauseGame);
    }
    
}
