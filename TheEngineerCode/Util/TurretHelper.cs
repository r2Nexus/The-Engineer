using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Orbs;

namespace TheEngineer.TheEngineerCode.Util;

public static class TurretHelper
{
    public static List<TOrb> GetOrbs<TOrb>(Player player)
    {
        return player.PlayerCombatState
            .OrbQueue
            .Orbs
            .OfType<TOrb>()
            .ToList();
    }

    public static async Task FireAllTurrets(
        PlayerChoiceContext choiceContext,
        Player player,
        Creature? target = null)
    {
        List<TurretOrb> turrets = GetOrbs<TurretOrb>(player);

        foreach (TurretOrb turret in turrets)
            await turret.Fire(choiceContext, target);
    }
}