using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement m_movement;
    [SerializeField] private PlayerWeaponHandler m_handler;
    [SerializeField] private SpriteRenderer m_model;
    
    public PlayerWeaponHandler WeaponHandler => m_handler;
    

    public void Freeze()
    {
        m_movement.SetCanMove(false);
        m_handler.SetCanUse(false);
    }

    public void UnFreeze()
    {
        m_movement.SetCanMove(true);
        m_handler.SetCanUse(true);
        m_model.enabled = true;
    }

    public void HideVisual()
    {
        m_model.enabled = false;
    }
}
