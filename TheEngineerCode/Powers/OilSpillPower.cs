using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheEngineer.TheEngineerCode.Powers;

public class OilSpillPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount < 0M && power.Owner.IsEnemy && power.Owner == Owner && (power is OilPower))
        {
            Flash();
            await CommonActions.Apply<WeakPower>(Owner,null,Amount);
            
            await PowerCmd.ModifyAmount(
                choiceContext,
                this,
                -Amount,
                null,
                null);
        }
    }
}