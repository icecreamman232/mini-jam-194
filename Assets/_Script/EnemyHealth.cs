using System.Collections;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private SpriteRenderer m_model;
    private MaterialPropertyBlock m_materialPropertyBlock;
    private readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");
    [SerializeField] private EnemyHealthBar m_healthBar;

    protected override void Start()
    {
        m_materialPropertyBlock = new MaterialPropertyBlock();
        base.Start();
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
}
