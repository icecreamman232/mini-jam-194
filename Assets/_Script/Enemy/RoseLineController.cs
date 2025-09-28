
using UnityEngine;

public class RoseLineController : MonoBehaviour
{
    [SerializeField] private LineRenderer m_lineRenderer;
    [SerializeField] private SpringJoint2D m_springJoint2D;
    
    private Transform m_target;
    private Vector2 m_targetPosition;
    private bool m_flyingHookToTarget;
    
    public void SetTarget(Transform target)
    {
        m_target = target;
        m_flyingHookToTarget = true;
        m_targetPosition = transform.position;
    }
    
    private void Update()
    {
        if(m_target == null) return;

        if (m_flyingHookToTarget)
        {
            m_targetPosition = Vector2.MoveTowards(m_targetPosition, m_target.position, 30f * Time.deltaTime);
            m_lineRenderer.SetPosition(0, transform.position);
            m_lineRenderer.SetPosition(1, m_targetPosition);
            if (Vector2.Distance(m_target.position, m_targetPosition) < 0.1f)
            {
                m_springJoint2D.connectedBody = m_target.GetComponent<Rigidbody2D>();
                m_flyingHookToTarget = false;
            }

        }
        else
        {
            m_lineRenderer.SetPosition(0, transform.position);
            m_lineRenderer.SetPosition(1, m_target.position);
        }
    }
}
