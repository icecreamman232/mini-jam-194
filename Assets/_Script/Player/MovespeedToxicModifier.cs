
public class MovespeedToxicModifier : ToxicModifier
{
    public override void Apply(PlayerController player)
    {
        player.Movement.ModifySpeed(-1);
    }
}
