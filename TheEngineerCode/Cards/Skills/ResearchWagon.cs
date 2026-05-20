using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.HoverTips;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class ResearchWagon() : TheEngineerCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const int BASE_CARDS = 2;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Wagon
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Science),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(BASE_CARDS)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> scienceCards = Owner.Character.CardPool
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Tags.Contains(TheEngineerCardTags.Science));

        List<CardModel> cards = CardFactory.GetDistinctForCombat(
                Owner,
                scienceCards,
                DynamicVars.Cards.IntValue,
                Owner.RunState.Rng.CombatCardGeneration)
            .ToList();

        foreach (CardModel card in cards)
        {
            if (IsUpgraded)
                CardCmd.Upgrade(card);

            card.SetToFreeThisTurn();

            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                Owner);
        }
    }
}