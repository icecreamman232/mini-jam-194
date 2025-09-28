
public class FrozenBulletModifier : ItemModifier
{
    private PlayerWeaponHandler m_weaponHandler;
    
    public FrozenBulletModifier(PlayerWeaponHandler weaponHandler)
    {
        m_weaponHandler = weaponHandler;
    }

    public override void Apply(float value)
    {
        m_weaponHandler.Weapon.SetFrozeChance(value);
        base.Apply(value);
    }
}
