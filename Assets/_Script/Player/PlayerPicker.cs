using System;
using UnityEngine;

public class PlayerPicker : MonoBehaviour
{
    [SerializeField] private LayerMask m_pickersMask;
    [SerializeField] private float m_pickRadius;
    private Collider2D[] m_pickableColliders;

    private void Start()
    {
        m_pickableColliders = new Collider2D[10];
    }

    private void Update()
    {
        Physics2D.OverlapCircleNonAlloc(transform.position, m_pickRadius,m_pickableColliders,m_pickersMask);

        if (m_pickableColliders.Length > 0)
        {
            for (int i = 0; i < m_pickableColliders.Length; i++)
            {
                if(m_pickableColliders[i] ==null) continue;
                var pickable = m_pickableColliders[i].GetComponent<IPickup>();
                pickable.Picking(this.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_pickRadius);
    }
}
