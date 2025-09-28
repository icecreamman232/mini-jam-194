
public class IncreaseKnockBackModifier : ItemModifier
{
    private PlayerWeaponHandler m_weaponHandler;
    
    public IncreaseKnockBackModifier(PlayerWeaponHandler weaponHandler)
    {
        m_weaponHandler = weaponHandler;
    }

    public override void Apply(float value)
    {
        m_weaponHandler.Weapon.ChangeKnockBackForce(value);
        base.Apply(value);
    }
}
