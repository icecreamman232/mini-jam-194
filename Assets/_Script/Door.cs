using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform m_model;
    [SerializeField] private Collider2D m_collider;
    
    private void ShowDoor()
    {
        m_model.gameObject.SetActive(true);
        m_model.localScale = Vector3.one * 0.5f;
        //m_model.
    }
}
