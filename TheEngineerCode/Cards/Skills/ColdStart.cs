using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
    private const string ExhaustKey = "Exhaust";

    private const decimal BASE_EXHAUST = 3m;
    private const int DRAW_PER_EXHAUST = 1;
    private const int PRODUCE_PER_EXHAUST = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ExhaustKey, BASE_EXHAUST),
        new CardsVar(DRAW_PER_EXHAUST),
        new ProduceVar(PRODUCE_PER_EXHAUST)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<CardModel> validCards = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(card => card != this)
            .ToList();

        if (validCards.Count <= 0) return;

        int maxToExhaust = (int)DynamicVars[ExhaustKey].BaseValue;
        int selectionCount = int.Min(maxToExhaust, validCards.Count);

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
            await CardCmd.Exhaust(choiceContext, card);

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