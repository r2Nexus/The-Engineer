using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Hooks;

namespace TheEngineer.TheEngineerCode.Powers;

public class EMPlantPower : TheEngineerPower, IOnChargeSpent
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public async Task OnChargeSpent(PlayerChoiceContext choiceContext, Player player, CardModel? spentCard, decimal amount,
        AbstractModel? causedBy)
    {
        if (player != Owner.Player)
            return;
        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            Amount,
            player);
    }
}