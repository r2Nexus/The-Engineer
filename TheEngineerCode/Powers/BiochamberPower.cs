using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;


public class BiochamberPower : TheEngineerPower, IOnConsumed
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OilPower>(2)
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnConsumed(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy)
    {
        Creature? target = CombatState.RunState.Rng.CombatTargets.NextItem(
            CombatState.HittableEnemies);

        if (target == null)
            return;

        await CommonActions.Apply<OilPower>(
            target,
            null,
            Amount);
    }
}