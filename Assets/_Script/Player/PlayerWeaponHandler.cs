using System;
using System.Linq;
using SGGames.Script.Core;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private Transform m_weaponPivot;
    [SerializeField] private PlayerWeapon m_weapon;
    [SerializeField] private Rigidbody2D m_rigidbody;

    [Header("Gun Model")] 
    [SerializeField] private ModelPositionData m_modelPositionData;
    [SerializeField] private SpriteRenderer m_barrelModel;
    [SerializeField] private SpriteRenderer m_stockModel;
    
    private bool m_canUse = true;
    
    private Vector2 m_aimDirection;
    private bool m_isFlipped;

    public PlayerWeapon Weapon => m_weapon;
    public Action OnShoot;
    
    private void Start()
    {
        ServiceLocator.GetService<InputManager>().OnAttackInputCallback = OnAttackInputCallback;
        ServiceLocator.GetService<InputManager>().OnReloadInputCallback = OnReloadInputCallback;
    }

    private void Update()
    {
        if(LevelManager.IsGamePaused) return;
        if (!m_canUse) return;
        if (!this.gameObject.activeInHierarchy) return;
        m_aimDirection = (InputManager.GetWorldMousePosition() - m_weapon.ShootingPivot.position).normalized;
        UpdateWeaponRotation();
    }

    public void SetCanUse(bool canUse)
    {
        m_canUse = canUse;
    }

    public void UpdateGunModel(ItemData itemData)
    {
        var modelData = m_modelPositionData.ModelDataList.FirstOrDefault(data => data.ItemID == itemData.ItemID);
        if(modelData == null) return;
        switch (modelData.Type)
        {
            case ModelType.Barrel:
                m_barrelModel.transform.localPosition = modelData.Position;
                m_barrelModel.sprite = modelData.Model;
                break;
            case ModelType.Stock:
                m_stockModel.transform.localPosition = modelData.Position;
                m_stockModel.sprite = modelData.Model;
                break;
            case ModelType.Gun:
                break;
        }
    }

    private void OnAttackInputCallback()
    {
        if (!m_canUse) return;
        if (m_weapon.IsReloading) return;
        if (m_weapon.Shoot(m_aimDirection))
        {
            OnShoot?.Invoke();
            m_rigidbody.AddForce(-m_aimDirection * (100f * m_weapon.RecoilForce));
        }
    }
    
    private void OnReloadInputCallback()
    {
        if (!m_canUse) return;
        if (m_weapon.IsReloading) return;
        if (!m_weapon.CanShoot) return;
        m_weapon.ManualReload();
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
            shouldFlip = angle < 90 && angle > -90;
        }
        else
        {
            // Original logic for right side
            shouldFlip = angle > 90 || angle < -90;
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
