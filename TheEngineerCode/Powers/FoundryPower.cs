using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public class FoundryPower : TheEngineerPower, IOnProduced
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OilPower>(2)
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnProduced(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialDestination destination,
        AbstractModel? causedBy)
    {
        if (amount < 2)
            return;

        if (DeferredOrbChannel.IsChannelResolving)
        {
            DeferredOrbChannel.Enqueue<TurretOrb>(choiceContext, player);
            return;
        }

        await OrbCmd.Channel<TurretOrb>(choiceContext, player);
    }
}