using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace TheEngineer.TheEngineerCode.Powers;

public class BeltFedPower : TheEngineerPower
{
    private const decimal DAMAGE_LOSS = 2m;
    private const int FIRE_COUNT = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public static bool IsActive(Player player)
        => player.Creature.GetPower<BeltFedPower>() != null;

    public static decimal ModifyTurretFireDamage(Player player, decimal baseDamage)
    {
        if (!IsActive(player))
            return baseDamage;

        return Math.Max(0M, baseDamage - DAMAGE_LOSS);
    }

    public static int GetTurretFireCount(Player player)
        => IsActive(player) ? FIRE_COUNT : 1;
}