using SGGames.Script.Core;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private SpriteRenderer m_model;
    private Vector2 m_moveDirection;

    private void Start()
    {
        ServiceLocator.GetService<InputManager>().OnMoveInputCallback += OnReceiveMovementInput;
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<InputManager>().OnMoveInputCallback -= OnReceiveMovementInput;
    }

    private void OnReceiveMovementInput(Vector2 moveDirection)
    {
        m_moveDirection = moveDirection;
        if (m_moveDirection.x != 0)
        {
            FlipModel(m_moveDirection.x < 0);
        }
    }

    private void FixedUpdate()
    {
        var deltaMovement = m_moveDirection * (m_speed * Time.fixedDeltaTime);
        m_rigidbody.MovePosition(m_rigidbody.position + deltaMovement);
    }

    private void FlipModel(bool isFlipped)
    {
        m_model.flipX = isFlipped;
    }
}
