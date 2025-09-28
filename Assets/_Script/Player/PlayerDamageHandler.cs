using System;
using SGGames.Script.Core;
using UnityEngine;

public class PlayerDamageHandler : DamageHandler
{
    private PlayerBullet m_bullet;

    private void Awake()
    {
        m_bullet = GetComponent<PlayerBullet>();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if(!LayerManager.IsInLayerMask(other.gameObject.layer, m_targetMask)) return;
        
        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(GetDamage(), this.gameObject, m_invulnerabilityDuration);
        }

        if (other.gameObject.TryGetComponent<IKnockback>(out var knockbackable))
        {
            var atkDirection = (other.transform.position - transform.position).normalized;
            knockbackable.ApplyKnockback(atkDirection * m_knockbackForce * 100f);
        }

        if (m_bullet.IsFrozen && other.gameObject.TryGetComponent<EnemyController>(out var controller))
        {
            controller.ApplyFroze(m_bullet.FrozeDuration);
        }
        
        OnHitTarget?.Invoke();
    }
}
