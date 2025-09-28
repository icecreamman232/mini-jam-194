using SGGames.Script.Core;
using UnityEngine;

public class FourHeadAI : EnemyAI
{
    [SerializeField] private EnemyHealth m_health;
    [SerializeField] private EnemyMovement m_movement;
    private Transform m_playerTransform;
    private Vector3 m_directionToPlayer;

    private void Start()
    {
        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
    }
    
    private void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if (!m_canThink) return;
        if(m_playerTransform == null) return;

        var lastestDirectionToPlayer = (m_playerTransform.position - transform.position).normalized;
        if (m_directionToPlayer != lastestDirectionToPlayer)
        {
            m_directionToPlayer = lastestDirectionToPlayer;
            m_movement.ChangeMoveDirection(lastestDirectionToPlayer);
        }
    }
}
