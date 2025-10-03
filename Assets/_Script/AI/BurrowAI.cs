using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int m_frameUpdatePathFinding = 120;
    private Transform m_playerTransform;
    private Vector3 m_directionToPlayer;
    private float m_timeSinceLastFollowing;
    private bool m_isFollowingPlayer;
    private bool m_isAttacking;
    private readonly int m_attackAnimParam = Animator.StringToHash("Attack");
    private const float k_AttackAnimDuration = 2.1f;
    private RoomPathStatus m_roomPathStatus;
    private List<Vector2> m_path;
    private int m_frameCounter;
    private int m_currentPathIndex;

    private void Start()
    {
        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
        m_isFollowingPlayer = true;
        m_currentPathIndex = 0;
        m_timeSinceLastFollowing = 0;
        m_health.SetNoDamage(true);
        m_roomPathStatus = ServiceLocator.GetService<RoomPathStatus>();
        if (m_roomPathStatus != null)
        {
            m_roomPathStatus.RegisterEnemy(transform);
        }
        m_path = m_roomPathStatus.FindPathToPlayer(transform.position);
    }
    
    private void OnDisable()
    {
        // Register this enemy with the pathfinding system
        if (m_roomPathStatus != null)
        {
            m_roomPathStatus.UnregisterEnemy(transform);
        }
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
            //Only update path every X frames
            m_frameCounter++;
            if (m_frameCounter >= m_frameUpdatePathFinding)
            {
                m_path = m_roomPathStatus.FindPathToPlayer(transform.position);
                m_frameCounter = 0;
                m_currentPathIndex = 0;
            }

            if (m_path != null && m_path.Count != 0)
            {
                //Move to next point in path
                if (Vector2.Distance((Vector3)m_path[m_currentPathIndex], transform.position) <= 0.1f)
                {
                    m_currentPathIndex++;
                    if (m_currentPathIndex >= m_path.Count)
                    {
                        m_path = m_roomPathStatus.FindPathToPlayer(transform.position);
                        m_currentPathIndex = 0;
                    }
                }  
            }
            
            m_timeSinceLastFollowing += Time.deltaTime;

            if (m_timeSinceLastFollowing >= m_followingTime)
            {
                m_timeSinceLastFollowing = 0;
                m_movement.ChangeMoveDirection(Vector2.zero);
                m_isFollowingPlayer = false;
                m_health.SetNoDamage(false);
                return;
            }
        
            var newDirectionToPlayer = ((Vector3)m_path[m_currentPathIndex] - transform.position).normalized;
            if (newDirectionToPlayer != m_directionToPlayer)
            {
                m_directionToPlayer = newDirectionToPlayer;
                m_movement.ChangeMoveDirection(m_directionToPlayer);
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
        m_path = m_roomPathStatus.FindPathToPlayer(transform.position);
        m_currentPathIndex = 0;
    }
}
