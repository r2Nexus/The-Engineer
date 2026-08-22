using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.GameInfo.Objects;

namespace TheEngineer.TheEngineerCode.Powers;
public sealed class TungstenCarbideThornsPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ThornsPower>()
    ];

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        ThornsPower? thorns = Owner.GetPower<ThornsPower>();

        if (thorns != null)
        {
            int amountToRemove = Math.Min(
                Amount,
                thorns.Amount);
            await PowerCmd.ModifyAmount(
                choiceContext,
                thorns,
                -amountToRemove,
                null,
                null,
                true
            );
        }
        await PowerCmd.Remove(this);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (applier?.Player != Owner.Player) return;
        if (power != this) return;
        if (amount < 0M) return;
        await CommonActions.Apply<ThornsPower>(Owner,null,amount,true);
    }
}