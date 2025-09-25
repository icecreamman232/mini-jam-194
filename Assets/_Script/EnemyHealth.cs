using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private EnemyHealthBar m_healthBar;
    
    protected override void UpdateHealthBar()
    {
        m_healthBar.UpdateBar(m_currentHealth, m_maxHealth);
        base.UpdateHealthBar();
    }
}
