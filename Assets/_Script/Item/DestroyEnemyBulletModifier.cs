using UnityEngine;

public class DestroyEnemyBulletModifier : ItemModifier
{
    private PlayerWeaponHandler m_weaponHandler;
    
    public DestroyEnemyBulletModifier(PlayerWeaponHandler weaponHandler)
    {
        m_weaponHandler = weaponHandler;
    }

    public override void Apply(float value)
    {
        m_weaponHandler.Weapon.SetPlayerBulletDestroyEnemyBullet();
        base.Apply(value);
    }
}
