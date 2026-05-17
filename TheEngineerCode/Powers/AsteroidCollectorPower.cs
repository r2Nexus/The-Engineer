using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards.Attacks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;
public sealed class AsteroidCollectorPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ProduceVar(Amount)
    ];

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel card = play.Card;

        if (card.Owner.Creature != Owner)
            return;

        if (card.Type != CardType.Attack)
            return;

        int produceAmount = Amount;
        
        // prevents the card also immediately triggering this
        if (card is AsteroidCollector)
            produceAmount--;

        if (produceAmount <= 0)
            return;

        Flash();

        await MaterialHelper.ProduceMaterial(
            Owner.Player,
            choiceContext,
            produceAmount,
            MaterialDestination.Hand,
            this);
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -Amount,
            null,
            null);
    }
}