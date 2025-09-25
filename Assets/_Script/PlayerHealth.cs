using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private PlayerHealthEvent m_healthEvent;
    private HealthEventData m_healthEventData = new HealthEventData();
    
    protected override void UpdateHealthBar()
    {
        m_healthEventData.CurrentHealth = m_currentHealth;
        m_healthEventData.MaxHealth = m_maxHealth;
        m_healthEvent.Raise(m_healthEventData);
        base.UpdateHealthBar();
    }

    protected override void Damage(float damage)
    {
        //Normalize damage for player since they have heart and only take 1 damage per hit
        if (damage > 0)
        {
            damage = 1;
        }
        base.Damage(damage);
    }
}
