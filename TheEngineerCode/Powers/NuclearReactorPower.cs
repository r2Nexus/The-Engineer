using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;


public class NuclearReactorPower : TheEngineerPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(Amount),
        new ConsumeVar(2*Amount)
    ];
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        bool consumed = await MaterialHelper.ConsumeMaterial(
            player,
            choiceContext,
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Stock,
            this);

        if (consumed)
        {
            await PlayerCmd.GainEnergy(
                DynamicVars.Energy.BaseValue,
                player);
        }
    }
}