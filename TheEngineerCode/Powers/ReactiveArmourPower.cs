using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheEngineer.TheEngineerCode.Orbs;

namespace TheEngineer.TheEngineerCode.Powers;


public class ReactiveArmourPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        Flash();
        for(var i = 0; i < Amount; i++) await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner.Player);
        await PowerCmd.ModifyAmount(choiceContext,this, -Amount, null, null);
    }
}