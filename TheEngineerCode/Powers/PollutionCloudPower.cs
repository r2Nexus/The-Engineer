using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public class PollutionCloudPower : TheEngineerPower, IOnProduced
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnProduced(PlayerChoiceContext choiceContext, Player player, int amount, MaterialDestination destination,
        AbstractModel? causedBy)
    {
        if(Owner.Player != player) return;
        if(amount <= 0) return;
        
        await CreatureCmd.Damage(choiceContext,CombatState.HittableEnemies, Amount,
            ValueProp.Unpowered, Owner);
    }
}