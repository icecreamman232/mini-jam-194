using System;
using SGGames.Script.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private LayerMask m_targetMask;
    [SerializeField] private float m_minDamage;
    [SerializeField] private float m_maxDamage;
    [SerializeField] private float m_invulnerabilityDuration = 0.3f;

    public Action OnHitTarget;
    
    private float GetDamage()
    {
        return Random.Range(m_minDamage, m_maxDamage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!LayerManager.IsInLayerMask(other.gameObject.layer, m_targetMask)) return;

        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(GetDamage(), this.gameObject, m_invulnerabilityDuration);
        }
        OnHitTarget?.Invoke();
    }
}
