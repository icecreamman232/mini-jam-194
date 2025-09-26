using SGGames.Script.Core;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private Animator m_animator;
    private Vector2 m_moveDirection;
    private readonly int m_runningBooleanAnimParam = Animator.StringToHash("Is Running");
    private bool m_canMove = true;
    
    private void Start()
    {
        ServiceLocator.GetService<InputManager>().OnMoveInputCallback += OnReceiveMovementInput;
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<InputManager>().OnMoveInputCallback -= OnReceiveMovementInput;
    }
    
    private void FixedUpdate()
    {
        if (!m_canMove) return;
        var deltaMovement = m_moveDirection * (m_speed * Time.fixedDeltaTime);
        m_rigidbody.MovePosition(m_rigidbody.position + deltaMovement);
    }
    
    public void SetCanMove(bool canMove)
    {
        m_canMove = canMove;
    }
    
    private void OnReceiveMovementInput(Vector2 moveDirection)
    {
        if (!m_canMove) return;
        m_moveDirection = moveDirection;
        if (m_moveDirection.x != 0)
        {
            FlipModel(m_moveDirection.x < 0);
        }
        m_animator.SetBool(m_runningBooleanAnimParam, m_moveDirection != Vector2.zero);
    }

    private void FlipModel(bool isFlipped)
    {
        m_model.flipX = isFlipped;
    }
}
