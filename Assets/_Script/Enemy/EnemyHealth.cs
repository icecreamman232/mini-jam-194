using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class EnemyHealth : Health
{
    [SerializeField] private bool m_isNoDamage;
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private RegisterEnemyEvent m_registerEnemyEvent;
    [SerializeField] private EnemyHealthBar m_healthBar;
    private MaterialPropertyBlock m_materialPropertyBlock;
    private readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");
    public Action OnDeath;

    protected override void Start()
    {
        m_materialPropertyBlock = new MaterialPropertyBlock();
        base.Start();
        m_registerEnemyEvent.Raise(new RegisteredEnemyData()
        {
            State = EnemyState.Alive,
            EnemyHealth = this
        });
    }

    public void SetNoDamage(bool isNoDamage)
    {
        m_isNoDamage = isNoDamage;
    }

    protected override bool CanTakeDamage()
    {
        if (m_isNoDamage) return false;
        
        return base.CanTakeDamage();
    }

    protected override void UpdateHealthBar()
    {
        m_healthBar.UpdateBar(m_currentHealth, m_maxHealth);
        base.UpdateHealthBar();
    }

    protected override IEnumerator OnInvulnerable(float duration)
    {
        m_isInvulnerable = true;
        var timeStop = Time.time + duration;

        while (Time.time < timeStop)
        {
            m_model.GetPropertyBlock(m_materialPropertyBlock);
            m_materialPropertyBlock.SetFloat(BlendAmount, 1);
            m_model.SetPropertyBlock(m_materialPropertyBlock);
            yield return new WaitForSeconds(0.08f);
            m_model.GetPropertyBlock(m_materialPropertyBlock);
            m_materialPropertyBlock.SetFloat(BlendAmount, 0);
            m_model.SetPropertyBlock(m_materialPropertyBlock);
            yield return new WaitForSeconds(0.08f);
        }
        
        //Ensure the effect of shader is removed
        m_model.GetPropertyBlock(m_materialPropertyBlock);
        m_materialPropertyBlock.SetFloat(BlendAmount, 0);
        m_model.SetPropertyBlock(m_materialPropertyBlock);
        
        m_isInvulnerable = false;
    }

    protected override void Die()
    {
        m_registerEnemyEvent.Raise(new RegisteredEnemyData
        {
            State = EnemyState.Dead,
            EnemyHealth = this
        });
        base.Die();
        OnDeath?.Invoke();
    }
}
