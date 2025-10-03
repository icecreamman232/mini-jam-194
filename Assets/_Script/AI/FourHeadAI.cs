using System.Collections.Generic;
using SGGames.Script.Core;
using UnityEngine;

public class FourHeadAI : EnemyAI
{
    [SerializeField] private EnemyHealth m_health;
    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private int m_frameUpdatePathFinding = 60;
    private Transform m_playerTransform;
    private Vector3 m_directionToPlayer;
    //Path finding
    private RoomPathStatus m_roomPathStatus;
    private List<Vector2> m_path;
    private int m_frameCounter;
    private int m_currentPathIndex;

    private void Start()
    {
        m_roomPathStatus = ServiceLocator.GetService<RoomPathStatus>();
        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
        // Register this enemy with the pathfinding system
        if (m_roomPathStatus != null)
        {
            m_roomPathStatus.RegisterEnemy(transform);
        }
        m_path = m_roomPathStatus.FindPathToPlayer(transform.position);
    }
    
    private void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if (!m_canThink) return;
        if(m_playerTransform == null) return;
        
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
    }
}
