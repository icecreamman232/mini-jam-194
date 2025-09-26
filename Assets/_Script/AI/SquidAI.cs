using System.Collections;
using SGGames.Script.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class SquidAI : EnemyAI
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private Weapon m_weapon;
    [SerializeField] private float m_rangeDetect;
    [SerializeField] private float m_distanceToShoot;
    [SerializeField] private LayerMask m_obstacleMask;
    private Transform m_player;
    private bool m_isFollowingPlayer;
    private Vector3 m_directionToPlayer;
    private bool m_isWaitingForShoot;
    private Vector2 m_shootDirection;
    private readonly int m_shootAnimParam = Animator.StringToHash("Trigger Attack");
    private const float k_ShootAnimDuration = 0.9f;
    
    private void Start()
    {
        SetRandomMoveDirection();
    }

    public void Shoot()
    {
        m_weapon.Shoot(m_shootDirection);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (m_isFollowingPlayer) return;
        
        if(!LayerManager.IsInLayerMask(other.gameObject.layer, m_obstacleMask)) return;

        SetRandomMoveDirection();
    }

    private void Update()
    {
        if(!this.gameObject.activeSelf) return;
        if(!m_canThink) return;

        if (m_isFollowingPlayer)
        {
            var newDirectionToPlayer = (m_player.position - transform.position).normalized;
            if (newDirectionToPlayer != m_directionToPlayer)
            {
                m_directionToPlayer = newDirectionToPlayer;
                m_movement.ChangeMoveDirection(m_directionToPlayer);
            }
            
            var distanceToPlayer = Vector2.Distance(transform.position, m_player.position);
            if (distanceToPlayer <= m_distanceToShoot && !m_isWaitingForShoot)
            {
                var shootDirection = (m_player.position - transform.position).normalized;
                m_shootDirection = shootDirection;
                if(m_weapon.CanShoot)
                {
                    StartCoroutine(BeforeShooting());
                }
            }
        }
        else
        {
            if (CheckTargetInRange())
            {
                m_directionToPlayer = (m_player.position - transform.position).normalized;
                m_movement.ChangeMoveDirection(m_directionToPlayer);
                m_isFollowingPlayer = true;
            }
        }
    }

    private IEnumerator BeforeShooting()
    {
        m_isWaitingForShoot = true;
        m_animator.SetTrigger(m_shootAnimParam);
        yield return new WaitForSeconds(k_ShootAnimDuration);
        m_animator.Play("Idle");
        m_isWaitingForShoot = false;
    }

    private bool CheckTargetInRange()
    {
        var result = Physics2D.OverlapCircle(transform.position, m_rangeDetect, LayerMask.GetMask("Player"));
        if (result != null)
        {
            m_player = result.transform;
            return true;
        }

        return false;
    }
    
    private void SetRandomMoveDirection()
    {
        var direction = Random.insideUnitCircle;
      
        //If the raycast hits something, reverse the direction
        var checkCollision = Physics2D.Raycast(transform.position, direction, 0.5f, m_obstacleMask);
        if (checkCollision.collider != null)
        {
            direction = -direction;
        }
      
        m_movement.ChangeMoveDirection(direction.normalized);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_rangeDetect);
    }
}
