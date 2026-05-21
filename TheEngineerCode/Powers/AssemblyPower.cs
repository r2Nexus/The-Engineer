using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class AssemblyPower : TheEngineerPower, IOnConsumed
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnConsumed(PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy, CardPlay? play)
    {
        if (player != Owner.Player)
            return;

        if (Amount <= 0)
            return;

        if (amount <= 0)
            return;

        if (causedBy is not CardModel card)
            return;

        if (card.Owner != player)
            return;

        Flash();

        card.BaseReplayCount += 1;

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -1,
            Owner,
            null);
    }

    public Task OnConsumed(PlayerChoiceContext choiceContext, Player player, int amount, MaterialSource source, CardPlay? cardPlay,
        AbstractModel? causedBy)
    {
        throw new NotImplementedException();
    }
}