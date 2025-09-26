using System.Collections;
using SGGames.Script.Core;
using UnityEngine;

public class BurrowAI : EnemyAI
{
    [SerializeField] private EnemyHealth m_health;
    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private Weapon m_weaponLeft;
    [SerializeField] private Weapon m_weaponRight;
    [SerializeField] private Weapon m_weaponUp;
    [SerializeField] private Weapon m_weaponDown;
    [SerializeField] private Animator m_animator;
    [SerializeField] private float m_followingTime;
    private Transform m_playerTransform;
    private Vector3 m_directionToPlayer;
    private float m_timeSinceLastFollowing;
    private bool m_isFollowingPlayer;
    private bool m_isAttacking;
    private readonly int m_attackAnimParam = Animator.StringToHash("Attack");
    private const float k_AttackAnimDuration = 2.1f;
    

    private void Start()
    {
        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
        m_isFollowingPlayer = true;
        m_timeSinceLastFollowing = 0;
        m_health.SetNoDamage(true);
    }

    public void Shoot()
    {
        m_weaponLeft.Shoot(Vector2.left);
        m_weaponRight.Shoot(Vector2.right);
        m_weaponUp.Shoot(Vector2.up);
        m_weaponDown.Shoot(Vector2.down);
    }

    private void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if (!m_canThink) return;

        if (m_isFollowingPlayer)
        {
            m_timeSinceLastFollowing += Time.deltaTime;

            if (m_timeSinceLastFollowing >= m_followingTime)
            {
                m_timeSinceLastFollowing = 0;
                m_movement.ChangeMoveDirection(Vector2.zero);
                m_isFollowingPlayer = false;
                m_health.SetNoDamage(false);
                return;
            }
        
            var lastestDirectionToPlayer = (m_playerTransform.position - transform.position).normalized;
            if (m_directionToPlayer != lastestDirectionToPlayer)
            {
                m_directionToPlayer = lastestDirectionToPlayer;
                m_movement.ChangeMoveDirection(lastestDirectionToPlayer);
            }
        }
        else
        {
            if (!m_isAttacking)
            {
                StartCoroutine(OnPlayingAttack());
            }
        }
    }

    private IEnumerator OnPlayingAttack()
    {
        m_isAttacking = true;
        m_animator.SetTrigger(m_attackAnimParam);
        yield return new WaitForSeconds(k_AttackAnimDuration);
        m_animator.Play("Idle");
        m_isAttacking = false;
        m_isFollowingPlayer = true;
        m_health.SetNoDamage(true);
    }
}
