using SGGames.Script.Core;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private Transform m_weaponPivot;
    [SerializeField] private PlayerWeapon m_weapon;
    private Vector2 m_aimDirection;
    private bool m_isFlipped;
    
    private void Start()
    {
        ServiceLocator.GetService<InputManager>().OnAttackInputCallback += OnAttackInputCallback;
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<InputManager>().OnAttackInputCallback -= OnAttackInputCallback;
    }

    private void Update()
    {
        if (!this.gameObject.activeInHierarchy) return;
        m_aimDirection = InputManager.GetWorldMousePosition();
        UpdateWeaponRotation();
    }

    private void OnAttackInputCallback()
    {
        
    }

    private void UpdateWeaponRotation()
    {
        var angle = Mathf.Atan2(m_aimDirection.y, m_aimDirection.x) * Mathf.Rad2Deg;
        
        // Check if weapon is positioned on the left side (negative x)
        bool weaponOnLeftSide = m_weapon.transform.localPosition.x < 0;
        
        bool shouldFlip;
        
        if (weaponOnLeftSide)
        {
            // When weapon is on left side, flip the logic
            shouldFlip = angle < 60 && angle > -60;
        }
        else
        {
            // Original logic for right side
            shouldFlip = angle > 60 || angle < -60;
        }

        if (shouldFlip != m_isFlipped)
        {
            m_isFlipped = shouldFlip;
            var weaponScale = m_weapon.transform.localScale;
            weaponScale.y *= -1;
            m_weapon.transform.localScale = weaponScale;
        }
        
        // Adjust the direction for pivot rotation based on weapon position
        Vector2 pivotDirection = weaponOnLeftSide ? -m_aimDirection : m_aimDirection;
        m_weaponPivot.right = pivotDirection;
    }
}
