using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour , IKnockback
{
    [SerializeField] private bool m_canMove;
    [SerializeField] private bool m_canKnockBack = true;
    [SerializeField] private float m_speed;
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private Animator m_animator;
    
    private HashSet<int> m_animatorParamHashes = new HashSet<int>();
    private Vector2 m_moveDirection;
    private readonly int m_RunningBooleanAnimParam = Animator.StringToHash("Is Running");
    
    public Vector2 MoveDirection => m_moveDirection;
    
    public float Speed => m_speed;
    
    private void Start()
    {
        foreach(var animatorParam in m_animator.parameters)
        {
            m_animatorParamHashes.Add(animatorParam.nameHash);
        }
    }

    public void SetCanMove(bool canMove)
    {
        m_canMove = canMove;
    }

    public void SetSpeed(float speed)
    {
        m_speed = speed;
    }
    
    public void ChangeMoveDirection(Vector2 moveDirection)
    {
        m_moveDirection = moveDirection;
        if (moveDirection.x != 0)
        {
            FlipModel(moveDirection.x < 0);
        }

        if (m_animatorParamHashes.Contains(m_RunningBooleanAnimParam))
        {
            m_animator.SetBool(m_RunningBooleanAnimParam, m_moveDirection != Vector2.zero);
        }
    }
    
    private void FixedUpdate()
    {
        if (!m_canMove) return;
        m_rigidbody.MovePosition(m_rigidbody.position + m_moveDirection * (m_speed * Time.fixedDeltaTime));
    }

    private void FlipModel(bool isFlipped)
    {
        m_model.flipX = isFlipped;
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (!m_canMove) return;
        if (!m_canKnockBack) return;
        m_rigidbody.AddForce(force);
    }
}
