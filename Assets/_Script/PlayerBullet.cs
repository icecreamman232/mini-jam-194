using System.Collections;
using UnityEngine;

public class PlayerBullet : Bullet
{
    [SerializeField] private BoxCollider2D m_boxCollider2D;
    [SerializeField] private Animator m_animator;
    
    private readonly int m_ExplodeAnimParam = Animator.StringToHash("Explode Trigger");
    private readonly float m_explodeAnimDuration = 0.45f;
    private bool m_canDestroyEnemyBullet;

    private void OnEnable()
    {
        m_animator.Play("Idle");
    }

    public void SetDestroyEnemyBullet()
    {
        m_canDestroyEnemyBullet = true;
    }

    public void ModifyDamage(float value)
    {
        m_damageHandler.UpdateDamage(value);
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
}
