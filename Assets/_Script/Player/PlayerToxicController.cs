using UnityEngine;

public class PlayerToxicController : MonoBehaviour
{
    [SerializeField] private PlayerCollectToxicEvent m_collectToxicEvent;
    [SerializeField] private int m_toxicLevel = 1;
    [SerializeField] private float m_currentToxic;
    [SerializeField] private float m_maxToxic;

    private void Start()
    {
        m_collectToxicEvent.AddListener(AddToxic);
    }

    private void OnDestroy()
    {
        m_collectToxicEvent.RemoveListener(AddToxic);
    }

    private void AddToxic(float amount)
    {
        m_currentToxic += amount;
        if (m_currentToxic >= m_maxToxic)
        {
            float excessToxic = m_currentToxic - m_maxToxic;
            UpgradeToxicLevel(excessToxic > 0 ? excessToxic : 0);
        }
    }

    private void UpgradeToxicLevel(float excessToxic)
    {
        m_toxicLevel++;
        m_maxToxic += 3;
        m_currentToxic += excessToxic;
        if (m_currentToxic >= m_maxToxic)
        {
            float excessToxicAmount = m_currentToxic - m_maxToxic;
            UpgradeToxicLevel(excessToxicAmount > 0 ? excessToxicAmount : 0);
        }
    }
}
