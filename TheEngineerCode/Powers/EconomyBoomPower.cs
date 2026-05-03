using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using TheEngineer.TheEngineerCode.Orbs;

namespace TheEngineer.TheEngineerCode.Powers;


public class EconomyBoomPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
            return;
        for (var i = 0; i < Amount; i++)
            await OrbCmd.Channel<MinerOrb>(new ThrowingPlayerChoiceContext(), Owner.Player);
    }
}