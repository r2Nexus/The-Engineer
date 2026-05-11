using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class ResiduePower : TheEngineerPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (Amount < 2)
            return;

        decimal oilToApply = Math.Floor(Amount / 2m);
        decimal residueToSpend = oilToApply * 2m;

        if (oilToApply <= 0)
            return;

        Flash();

        await PowerCmd.Apply<OilPower>(
            choiceContext,
            Owner,
            oilToApply,
            null,
            null);

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -residueToSpend,
            null,
            null);
    }
}