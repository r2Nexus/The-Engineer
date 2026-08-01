using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Powers;

public class MineFieldPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if(power != this) return;
        if (Owner.Player == null) return;
        if(power.Owner != Owner) return;
        
        if (amount > 0 ) await OrbCmd.AddSlots(Owner.Player, (int)amount);
        if (amount < 0 ) OrbCmd.RemoveSlots(Owner.Player, (int)-amount);
    }
    
    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner.Player)
            return;

        await PowerCmd.ModifyAmount(choiceContext, this, -Amount, null, null);
    }
}