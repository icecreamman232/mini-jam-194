
public class RandomTeleportToxicModifier : ToxicModifier
{
    private PlayerTeleport m_teleport;
    
    public RandomTeleportToxicModifier(PlayerTeleport teleport)
    {
        m_teleport = teleport;
    }

    public override void Apply(PlayerController player)
    {
        m_teleport.SetUnlock();
        base.Apply(player);
    }
}
