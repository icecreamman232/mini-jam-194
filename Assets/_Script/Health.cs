using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] protected float m_maxHealth;
    [SerializeField] protected float m_currentHealth;
    
    protected bool m_isInvulnerable;

    protected virtual void Start()
    {
        m_currentHealth = m_maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage, GameObject attacker, float invulnerabilityTime)
    {
        if (!CanTakeDamage()) return;

        Damage(damage);
        
        AfterTookDamage(invulnerabilityTime);
    }

    protected virtual bool CanTakeDamage()
    {
        if (m_isInvulnerable) return false;
        if (m_currentHealth <= 0) return false;
        
        return true;
    }
    
    protected virtual void Damage(float damage)
    {
        m_currentHealth -= damage;
        UpdateHealthBar();
    }

    protected virtual void AfterTookDamage(float invulnerabilityTime)
    {
        if (m_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(OnInvulnerable(invulnerabilityTime));
        }
    }

    protected virtual void UpdateHealthBar()
    {
        
    }

    protected virtual void Die()
    {
        m_isInvulnerable = true;
        this.gameObject.SetActive(false);
    }

    protected virtual IEnumerator OnInvulnerable(float duration)
    {
        yield return null;
    }
}