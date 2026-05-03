using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public class YellowInserterPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new ProduceVar(1)
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        Flash();
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, player);
        await PowerCmd.ModifyAmount(choiceContext,this, -1, null, null);
        await MaterialHelper.ProduceMaterial(
            player,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand,
            this);
    }
}