using UnityEngine;
using UnityEngine.UI;

public class HealthSlot : MonoBehaviour
{
    [SerializeField] private Image m_emptyImage;
    [SerializeField] private Image m_fullImage;

    public void Activate()
    {
        m_emptyImage.gameObject.SetActive(false);
        m_fullImage.gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        m_emptyImage.gameObject.SetActive(true);
        m_fullImage.gameObject.SetActive(false);
    }
}
