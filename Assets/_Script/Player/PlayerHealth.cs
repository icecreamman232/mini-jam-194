using System.Collections;
using SGGames.Script.Core;
using SGGames.Scripts.System;
using UnityEngine;

public class PlayerHealth : Health
{
    #if UNITY_EDITOR
    [SerializeField] private bool m_godMode;
    #endif
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private PlayerHealthEvent m_healthEvent;
    [SerializeField] private GameEvent m_gameEvent;
    private HealthEventData m_healthEventData = new HealthEventData();
    private CameraController m_cameraController;
    private MaterialPropertyBlock m_materialPropertyBlock;
    private static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");

    protected override void Start()
    {
        m_cameraController = ServiceLocator.GetService<CameraController>();
        m_materialPropertyBlock = new MaterialPropertyBlock();
        base.Start();
    }

    protected override void UpdateHealthBar()
    {
        m_healthEventData.CurrentHealth = m_currentHealth;
        m_healthEventData.MaxHealth = m_maxHealth;
        m_healthEvent.Raise(m_healthEventData);
        base.UpdateHealthBar();
    }

    #if UNITY_EDITOR
    protected override bool CanTakeDamage()
    {
        if (m_godMode) return false;
        return base.CanTakeDamage();
    }
    #endif

    protected override void Damage(float damage)
    {
        //Normalize damage for player since they have heart and only take 1 damage per hit
        if (damage > 0)
        {
            damage = 1;
        }
        base.Damage(damage);
        m_cameraController.TriggerShake(0.2f, 0.2f);
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
        m_gameEvent.Raise(GameEventType.GameOver);
        base.Die();
    }
}
