using UnityEngine;

public class PlayerToxicController : MonoBehaviour
{
    [SerializeField] private PlayerCollectToxicEvent m_collectToxicEvent;
    [SerializeField] private int m_toxicLevel = 1;
    [SerializeField] private float m_currentToxic;
    [SerializeField] private float m_maxToxic;
    
    private ToxicEventData m_toxicEventData = new ToxicEventData();
    
    public void AddToxic(float amount)
    {
        m_currentToxic += amount;
        UpdateUI();
        if (m_currentToxic >= m_maxToxic)
        {
            UpgradeToxicLevel();
            UpdateUI();
        }
    }

    private void UpgradeToxicLevel()
    {
        m_toxicLevel++;
        m_maxToxic += 3;
        m_currentToxic = 0;
    }

    private void UpdateUI()
    {
        m_toxicEventData.ToxicLevel = m_toxicLevel;
        m_toxicEventData.CurrentToxic = m_currentToxic;
        m_toxicEventData.MaxToxic = m_maxToxic;
        m_collectToxicEvent.Raise(m_toxicEventData);
    }
}
