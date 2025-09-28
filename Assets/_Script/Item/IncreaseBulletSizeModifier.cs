using UnityEngine;

public class IncreaseBulletSizeModifier : ItemModifier
{
    private PlayerWeaponHandler m_weaponHandler;
    
    public IncreaseBulletSizeModifier(PlayerWeaponHandler weaponHandler)
    {
        m_weaponHandler = weaponHandler;
    }

    public override void Apply(float value)
    {
        m_weaponHandler.Weapon.UpdateBulletSize(value);
        base.Apply(value);
    }
}
