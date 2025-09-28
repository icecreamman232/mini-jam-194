using SGGames.Script.Core;
using UnityEngine;

public class SwordAI : EnemyAI
{
    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private float m_restDuration;
    [SerializeField] private float m_movingDuration;
    [SerializeField] private LayerMask m_obstacleMask;
    [SerializeField] private Weapon[] m_weaponList;

    private bool m_canShoot;
    private float m_timer;
    private bool m_isRest;
    private void Start()
    {
        m_timer = 0;
        m_isRest = false;
        SetRandomMoveDirection();
    }
   
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!LayerManager.IsInLayerMask(other.gameObject.layer, m_obstacleMask)) return;
      
        m_timer = 0;
        m_isRest = true;
        //Stop moving
        m_movement.ChangeMoveDirection(Vector2.zero);
    }

    private void Update()
    {
        if (!this.gameObject.activeSelf) return;
        if(!m_canThink) return;
      
        if (m_isRest)
        {
            UpdateResting();
        }
        else
        {
            UpdateMoving();
        }
    }

    private void UpdateResting()
    {
        if (m_canShoot)
        {
            var angleStep = 360 / m_weaponList.Length;
            for (int i = 0; i < m_weaponList.Length; i++)
            {
                var angle = i * angleStep;
                var rad = angle * Mathf.Deg2Rad;
                var direction= new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                m_weaponList[i].Shoot(direction);
            }
            m_canShoot = false;
        }
        m_timer += Time.deltaTime;
        if (m_timer >= m_restDuration)
        {
            m_timer = 0;
            m_isRest = false;
            SetRandomMoveDirection();
        }
    }

    private void UpdateMoving()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_restDuration)
        {
            m_timer = 0;
            m_isRest = true;
            m_canShoot = true;
            //Stop moving
            m_movement.ChangeMoveDirection(Vector2.zero);
        }
    }

    private void SetRandomMoveDirection()
    {
        var direction = Random.insideUnitCircle;
      
        var checkCollision = Physics2D.Raycast(transform.position, direction, 0.5f, m_obstacleMask);
        if (checkCollision.collider != null)
        {
            direction = -direction;
        }
      
        m_movement.ChangeMoveDirection(direction.normalized);
    }
    
}
