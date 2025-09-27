using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement m_movement;
    [SerializeField] private PlayerWeaponHandler m_handler;
    [SerializeField] private PlayerHealth m_health;
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private SpriteRenderer m_weaponModel;
    
    public PlayerMovement Movement => m_movement;
    public PlayerWeaponHandler WeaponHandler => m_handler;
    public PlayerHealth Health => m_health;

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
        m_weaponModel.gameObject.SetActive(true);
    }

    public void HideVisual()
    {
        m_model.enabled = false;
        m_weaponModel.gameObject.SetActive(false);
    }
}
