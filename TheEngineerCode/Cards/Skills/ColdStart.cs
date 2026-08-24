using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class ColdStart() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const string DiscardKey = "Discard";

    private const decimal BASE_DISCARD = 3m;
    private const int DRAW_PER_DISCARD = 1;
    private const int PRODUCE_PER_DISCARD = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(DiscardKey, BASE_DISCARD),
        new CardsVar(DRAW_PER_DISCARD),
        new ProduceVar(PRODUCE_PER_DISCARD)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Material>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        List<CardModel> validCards = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(card => card != this)
            .ToList();

        if (validCards.Count <= 0) return;

        int maxToDiscard = (int)DynamicVars[DiscardKey].BaseValue;
        int selectionCount = int.Min(maxToDiscard, validCards.Count);

        CardSelectorPrefs prefs = new(SelectionScreenPrompt, 0, selectionCount)
        {
            PretendCardsCanBePlayed = true
        };

        List<CardModel> selectedCards = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                prefs,
                card => card != this,
                this))
            .ToList();

        foreach (CardModel card in selectedCards)
        {
            await CardCmd.Discard(choiceContext, card);

            await CardPileCmd.Draw(
                choiceContext,
                DynamicVars.Cards.BaseValue,
                Owner);

            await MaterialHelper.ProduceMaterial(
                Owner,
                choiceContext,
                (int)DynamicVars.Produce().BaseValue,
                MaterialDestination.Hand,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}