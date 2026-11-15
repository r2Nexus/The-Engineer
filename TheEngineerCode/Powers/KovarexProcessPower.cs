using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Hooks;

namespace TheEngineer.TheEngineerCode.Powers;

public class KovarexProcessPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Player == Owner.Player) return;
        if(!cardPlay.IsFirstInSeries) return;
        
        int cardsPlayed = CombatManager.Instance.History.CardPlaysStarted.Count(e =>
            e.Actor == cardPlay.Card.Owner.Creature &&
            e.CardPlay.IsFirstInSeries &&
            e.HappenedThisTurn(CombatState)
        );
        
        if (cardsPlayed > Amount)
            return;
        
        var myCard = cardPlay.Card.CreateCloneForPlayer(Owner.Player!);
        myCard.AddKeyword(TheEngineerKeyWords.Material);

        await CardPileCmd.AddGeneratedCardToCombat(
            myCard,
            PileType.Hand,
            Owner.Player);
    }
}