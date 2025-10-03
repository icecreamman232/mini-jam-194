using System.Collections;
using System.Collections.Generic;
using SGGames.Script.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class SquidAI : EnemyAI
{
    [SerializeField] private BoxCollider2D m_collider;
    [SerializeField] private Animator m_animator;
    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private Weapon m_weapon;
    [SerializeField] private float m_rangeDetect;
    [SerializeField] private float m_distanceToShoot;
    [SerializeField] private LayerMask m_obstacleMask;
    [SerializeField] private int m_frameUpdatePathFinding = 120;
    
    private Transform m_player;
    private bool m_isFollowingPlayer;
    private Vector3 m_directionToPlayer;
    private bool m_isWaitingForShoot;
    private Vector2 m_shootDirection;
    private readonly int m_shootAnimParam = Animator.StringToHash("Trigger Attack");
    private const float k_ShootAnimDuration = 0.9f;
    private RoomPathStatus m_roomPathStatus;
    private List<Vector2> m_path;
    private int m_frameCounter;
    private int m_currentPathIndex;
    
    private void Start()
    {
        SetRandomMoveDirection();
        m_roomPathStatus = ServiceLocator.GetService<RoomPathStatus>();
        // Register this enemy with the pathfinding system
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
                     
            var newDirectionToPlayer = ((Vector3)m_path[m_currentPathIndex] - transform.position).normalized;
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
                m_path = m_roomPathStatus.FindPathToPlayer(transform.position);
                m_movement.ChangeMoveDirection(m_directionToPlayer);
                m_isFollowingPlayer = true;
            }
            
            var checkCollision = Physics2D.Raycast(transform.position, m_movement.MoveDirection, 0.5f, m_obstacleMask);
            if (checkCollision.collider != null)
            {
                SetRandomMoveDirection();
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
        if(m_path == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_rangeDetect);
        
        // Test pathfinding from mouse position to player
        Vector3 mouseWorldPos = transform.position;
        if (mouseWorldPos != Vector3.zero)
        {
            // Draw mouse position
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(mouseWorldPos, 0.3f);
        
            // Find and draw path
            if (m_path.Count > 0)
            {
                // Draw path lines
                Gizmos.color = Color.magenta;
                Vector3 previousPos = mouseWorldPos;
            
                for (int i = 0; i < m_path.Count; i++)
                {
                    Vector3 currentPos = new Vector3(m_path[i].x, m_path[i].y, mouseWorldPos.z);
                
                    // Draw line from previous to current
                    Gizmos.DrawLine(previousPos, currentPos);
                
                    // Draw path point
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(currentPos, 0.2f);
                
                    previousPos = currentPos;
                    Gizmos.color = Color.magenta;
                }
            }
        
            // Draw mouse grid cell outline
            var mouseCell = m_roomPathStatus.WorldToCell(mouseWorldPos);
            if (mouseCell.x >= 0 && mouseCell.x < m_roomPathStatus.m_roomWidth && mouseCell.y >= 0 && mouseCell.y < m_roomPathStatus.m_roomHeight)
            {
                Gizmos.color = Color.cyan;
                var mouseCellWorldPos = m_roomPathStatus.CellToWorld(mouseCell);
                Gizmos.DrawWireCube(mouseCellWorldPos, Vector3.one * 1.2f);
            }
        }
    }
}
