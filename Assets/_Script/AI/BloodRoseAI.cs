using System;
using UnityEngine;

public class BloodRoseAI : EnemyAI
{
    [SerializeField] private float m_rangeHook;
    [SerializeField] private RoseLineController m_roseLineController;
    [SerializeField] private EnemyHealth m_health;
    [SerializeField] private Animator m_animator;
    [SerializeField] private BoxCollider2D m_collider;
    
    private readonly int m_AttackAnimParam = Animator.StringToHash("Attack");
    
    private bool m_isHooked;
    private Collider2D m_playerCollider;

    private void Start()
    {
        m_health.SetNoDamage(true);
    }

    private void Update()
    {
        if (m_isHooked) return;
        
        m_playerCollider = Physics2D.OverlapCircle(transform.position, m_rangeHook, LayerMask.GetMask("Player"));
        if (m_playerCollider != null)
        {
            m_animator.SetBool(m_AttackAnimParam, true);
            m_collider.size = new Vector2(1,1);
            m_roseLineController.SetTarget(m_playerCollider.transform);
            m_health.SetNoDamage(false);
            m_isHooked = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_rangeHook);
    }
}
