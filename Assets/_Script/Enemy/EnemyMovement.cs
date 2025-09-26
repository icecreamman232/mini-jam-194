using UnityEngine;

public class EnemyMovement : MonoBehaviour , IKnockback
{
    [SerializeField] private bool m_canMove;
    [SerializeField] private float m_speed;
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private Animator m_animator;
    
    private Vector2 m_moveDirection;
    private readonly int m_RunningBooleanAnimParam = Animator.StringToHash("Is Running");

    public void SetCanMove(bool canMove)
    {
        m_canMove = canMove;
    }
    
    public void ChangeMoveDirection(Vector2 moveDirection)
    {
        if (!m_canMove) return;
        m_moveDirection = moveDirection;
        if (moveDirection.x != 0)
        {
            FlipModel(moveDirection.x < 0);
        }
        m_animator.SetBool(m_RunningBooleanAnimParam, m_moveDirection != Vector2.zero);
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
        m_rigidbody.AddForce(force);
    }
}
