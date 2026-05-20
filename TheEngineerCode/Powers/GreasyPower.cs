using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheEngineer.TheEngineerCode.Powers;


public class GreasyPower : TheEngineerPower
{
    private const int DRAW_PER_TRIGGER = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(DRAW_PER_TRIGGER)
    ];

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount >= 0M) return;
        if (power is not OilPower) return;
        if (power.Owner != Owner) return;
        if (!Owner.IsEnemy) return;
        if (applier?.Player == null) return;

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            DRAW_PER_TRIGGER,
            applier.Player);

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -1,
            applier,
            cardSource);
    }
}