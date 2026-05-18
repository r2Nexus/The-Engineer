using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public class NuclearReactorPower : TheEngineerPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<NuclearReactorPower>(
            "MaterialAmount",
            power => (power.Amount * 2).ToString())
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStartLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player.Creature != Owner)
            return;

        int consumeAmount = Amount * 2;

        bool consumed = await MaterialHelper.ConsumeMaterial(
            player,
            choiceContext,
            consumeAmount,
            MaterialSource.Stock,
            this);

        if (consumed)
        {
            await PlayerCmd.GainEnergy(
                Amount,
                player);
        }
    }
}