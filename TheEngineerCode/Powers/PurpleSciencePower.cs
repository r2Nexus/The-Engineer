using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public class PurpleSciencePower: TheEngineerPower, IOnProduced
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [];
    
    public bool hasHappenedThisTurn = false;

    public async Task OnProduced(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialDestination destination,
        AbstractModel? causedBy)
    {
        if (Owner != player.Creature) return;
        if (causedBy == this)
            return;
        Flash();
        if (!hasHappenedThisTurn)
        {
            hasHappenedThisTurn = true;
            await MaterialHelper.ProduceMaterial(
                Owner.Player,
                choiceContext,
                Amount,
                MaterialDestination.Hand,
                this);
        }
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        hasHappenedThisTurn = false;
    }
}