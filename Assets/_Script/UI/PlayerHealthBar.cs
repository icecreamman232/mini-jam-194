using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealthEvent m_playerHealthEvent;
    [SerializeField] private HealthSlot[] m_healthSlots;

    private void Awake()
    {
        m_playerHealthEvent.AddListener(UpdateHealthBar);
        foreach (var slot in m_healthSlots)
        {
            slot.Activate();
        }
    }

    private void OnDestroy()
    {
        m_playerHealthEvent.RemoveListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(HealthEventData eventData)
    {
        for (int i = 0; i < m_healthSlots.Length; i++)
        {
            if (i >= eventData.CurrentHealth)
            {
                m_healthSlots[i].Deactivate();
            }
        }
    }
}
