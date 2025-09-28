
using UnityEngine;

public class DamageModifier : ItemModifier
{
    private PlayerWeaponHandler m_weaponHandler;
    
    public DamageModifier(PlayerWeaponHandler weaponHandler)
    {
        m_weaponHandler = weaponHandler;
    }

    public override void Apply(float value)
    {
        m_weaponHandler.Weapon.ModifyBulletDamage(value);
        Debug.Log($"Apply damage modifier {value}");
        base.Apply(value);
    }
}
