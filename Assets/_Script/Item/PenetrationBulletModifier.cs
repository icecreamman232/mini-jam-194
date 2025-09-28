
public class PenetrationBulletModifier :ItemModifier
{
    private PlayerWeaponHandler m_weaponHandler;
    
    public PenetrationBulletModifier(PlayerWeaponHandler weaponHandler)
    {
        m_weaponHandler = weaponHandler;
    }

    public override void Apply(float value)
    {
        m_weaponHandler.Weapon.SetPlayerBulletPenetrated();
        base.Apply(value);
    }
}
