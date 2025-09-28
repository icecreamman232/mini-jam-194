using System;
using System.Collections;
using SGGames.Script.Core;
using UnityEngine;
using Random = System.Random;

public class BossAI : EnemyAI
{
    [SerializeField] private EnemyHealth m_health;
    [SerializeField] private EnemyMovement m_movement;
    [SerializeField] private float m_delayBeforeShoot;
    [SerializeField] private Transform m_phase2YPivot;
    [SerializeField] private Transform m_leftLimit;
    [SerializeField] private Transform m_rightLimit;
    [SerializeField] private float m_phase2HealthThreshold;
    [SerializeField] private Weapon[] m_weaponList;
    [SerializeField] private Weapon[] m_phase2Weapons;

    private bool m_isMovingToSpot;
    private Transform m_playerTransform;
    private Vector3 m_directionToPlayer;
    private float m_delayBeforeShootTimer;
    private Action m_updatePhaseAction;

    private Vector2 m_targetPosition;
    private float m_movingPhase2Timer;
    private float m_moveSpeed = 3;
    private float MoveSpeed
    {
        get => m_moveSpeed;
        set => m_moveSpeed = value > 8 ? 8 : value;
    }

    private void Start()
    {
        m_playerTransform = ServiceLocator.GetService<LevelManager>().Player;
        m_updatePhaseAction = UpdatePhase1;
        m_delayBeforeShootTimer = m_delayBeforeShoot;
    }

    private void Update()
    {
        if (!m_canThink) return;

        m_updatePhaseAction?.Invoke();
    }

    
    private void UpdatePhase1()
    {
        var lastestDirectionToPlayer = (m_playerTransform.position - transform.position).normalized;
        if (m_directionToPlayer != lastestDirectionToPlayer)
        {
            m_directionToPlayer = lastestDirectionToPlayer;
            m_movement.ChangeMoveDirection(lastestDirectionToPlayer);
        }
        
        
        m_delayBeforeShootTimer -= Time.deltaTime;
        if (m_delayBeforeShootTimer <= 0)
        {
            m_delayBeforeShootTimer = m_health.CurrentPercentHealth <= 0.65f 
                ? UnityEngine.Random.Range(1, m_delayBeforeShoot) 
                : m_delayBeforeShoot;
            ShootGunPhase1();
        }

        if (m_health.CurrentPercentHealth <= m_phase2HealthThreshold && !m_isMovingToSpot)
        {
            StartCoroutine(MoveToPivotPhase2());    
        }
    }

    private void UpdatePhase2()
    {
        //Random change speed after random duration
        m_movingPhase2Timer -= Time.deltaTime;
        if (m_movingPhase2Timer <= 0)
        {
            m_movingPhase2Timer = UnityEngine.Random.Range(2f, 4f);
            MoveSpeed = UnityEngine.Random.Range(2f, 8f);

            //Shoot horizontally
            if (m_playerTransform.position.y >= 2)
            {
                var direction = m_playerTransform.position.x > 0 ? Vector2.right : Vector2.left;
                SetPhase2WeaponPosition(true);
                foreach (var weapon in m_phase2Weapons)
                {
                    weapon.Shoot(direction);
                }
            }
            else
            {
                SetPhase2WeaponPosition(false);
                foreach (var weapon in m_phase2Weapons)
                {
                    weapon.Shoot(Vector2.down);
                }
            }
        }
        
        
        //Moving loop
        transform.position = Vector2.MoveTowards(transform.position, m_targetPosition, MoveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, m_targetPosition) <= 0.1f)
        {
            transform.position = m_targetPosition;
            m_targetPosition = GetNextTarget().position;
        }
    }

    private void SetPhase2WeaponPosition(bool isHorizontal)
    {
        var gap = 2f;
        if (isHorizontal)
        {
            m_phase2Weapons[0].transform.localPosition = new Vector3(0, -gap/2, 0);
            m_phase2Weapons[1].transform.localPosition = new Vector3(0, gap/2, 0);
        }
        else
        {
            m_phase2Weapons[0].transform.localPosition = new Vector3(-gap/2, 0, 0);
            m_phase2Weapons[1].transform.localPosition = new Vector3(gap/2, 0, 0);
            
        }
    }

    private IEnumerator MoveToPivotPhase2()
    {
        m_isMovingToSpot = true;
        m_movement.SetCanMove(false);
        m_health.SetNoDamage(true);
        var targetPos = transform.position;
        targetPos.y = m_phase2YPivot.position.y;
        while (transform.position.y < targetPos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        
        m_updatePhaseAction = UpdatePhase2;
        
        m_movement.SetCanMove(true);
        m_health.SetNoDamage(false);

        
        m_movingPhase2Timer = UnityEngine.Random.Range(2f, 4f);
        
        m_isMovingToSpot = false;
    }
    
    private Transform GetNextTarget()
    {
        if (transform.position.x > 0)
        {
            return m_leftLimit;
        }
        return m_rightLimit;
    }

    private void ShootGunPhase1()
    {
        var angleStep = 360 / m_weaponList.Length;
        for (int i = 0; i < m_weaponList.Length; i++)
        {
            var angle = i * angleStep;
            var rad = angle * Mathf.Deg2Rad;
            var direction= new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            m_weaponList[i].Shoot(direction);
        }
    }
}
