using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    [SerializeField] private SpriteRenderer m_bulletSprite;
    [SerializeField] private BoxCollider2D m_boxCollider2D;
    [SerializeField] private Animator m_animator;
    
    private readonly int m_ExplodeAnimParam = Animator.StringToHash("Explode Trigger");
    private readonly float m_explodeAnimDuration = 0.45f;
    private int m_penetratedCount = 2;
    
    private bool m_canDestroyEnemyBullet;
    private bool m_canPenetrated;
    
    //Frozen effect
    private float m_chanceToFroze;
    private bool m_isFrozen;
    private float m_frozeDuration = 3;
    public float FrozeDuration => m_frozeDuration;
    public bool IsFrozen => m_isFrozen;
    
    private HashSet<Collider2D> m_hitTargets = new HashSet<Collider2D>();
    
    private void OnEnable()
    {
        m_isFrozen = false;
        m_animator.Play("Idle");
        m_hitTargets.Clear();
        var chance = UnityEngine.Random.Range(0f, 100f);
        m_isFrozen = chance <= m_chanceToFroze;
        m_bulletSprite.color = m_isFrozen 
            ? new Color(0.2389196f, 0.5321058f, 0.8584906f ) 
            : Color.white;
    }

    private void OnDisable()
    {
        m_penetratedCount = 2;
    }

    public void SetDestroyEnemyBullet()
    {
        m_canDestroyEnemyBullet = true;
    }

    public void UpdateBulletSize(float sizeMultiplier)
    {
        transform.localScale *= sizeMultiplier;
    }

    public void SetChanceToFroze(float chance)
    {
        m_chanceToFroze = chance;
    }

    public void ModifyDamage(float value)
    {
        m_damageHandler.UpdateDamage(value);
    }
    
    public void ChangeKnockBackForce(float force)
    {
        ((PlayerDamageHandler)m_damageHandler).ChangeKnockBackForce(force);
    }

    protected override void Update()
    {
        if (!m_isActivated) return;
        transform.position += transform.up * (m_speed * Time.deltaTime);
        
        if (m_canDestroyEnemyBullet && CheckCollisionWithEnemyBullet())
        {
            DestroyBullet();
        }
        
        m_travelledDistance = Vector2.Distance(transform.position, m_startPosition);
        if (m_travelledDistance >= m_range)
        {
            DestroyBullet();
        }
    }

    private bool CheckCollisionWithEnemyBullet()
    {
        var result = Physics2D.OverlapBox(transform.position, m_boxCollider2D.size, 0, LayerMask.GetMask("Enemy Bullet"));
        if (result != null)
        {
            var enemyBullet = result.GetComponent<Bullet>();
            enemyBullet.DestroyBulletImmediately();
            return true;
        }
        return false;
        
    }

    protected override void DestroyBullet()
    {
        StartCoroutine(OnBeforeDestroyingBullet());
    }

    private IEnumerator OnBeforeDestroyingBullet()
    {
        m_isActivated = false;
        m_animator.SetTrigger(m_ExplodeAnimParam);
        yield return new WaitForSeconds(m_explodeAnimDuration);
        base.DestroyBullet();
    }

    public void OnDamageHandlerHit(Collider2D target)
    {
        // Check if we've already hit this target
        if (m_hitTargets.Contains(target))
        {
            return; // Skip this hit - we've already damaged this target
        }

        // Add to hit targets to prevent hitting again
        m_hitTargets.Add(target);

        if (m_canPenetrated)
        {
            m_penetratedCount--;
 
            if (m_penetratedCount <= 0)
            {
                DestroyBullet();
            }
            // If penetratedCount > 0, continue flying and can hit more targets
        }
        else
        {
            // No penetration - destroy immediately
            DestroyBullet();
        }
    }
}
