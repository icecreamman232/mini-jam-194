using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float m_speed;
    [SerializeField] [Min(0)] protected float m_range;
    [SerializeField] private DamageHandler m_damageHandler;
    
    private Vector2 m_startPosition;
    private float m_travelledDistance;
    private bool m_isActivated;

    private void Awake()
    {
        m_damageHandler.OnHitTarget = DestroyBullet;
    }

    public void Spawn()
    {
        m_isActivated = true;
        m_travelledDistance = 0;
        m_startPosition = transform.position;
    }

    private void Update()
    {
        if (!m_isActivated) return;
        transform.position += transform.up * (m_speed * Time.deltaTime);
        m_travelledDistance = Vector2.Distance(transform.position, m_startPosition);
        if (m_travelledDistance >= m_range)
        {
            DestroyBullet();
        }
    }

    protected virtual void DestroyBullet()
    {
        m_isActivated = false;
        this.gameObject.SetActive(false);
    }
}
