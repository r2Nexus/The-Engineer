using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Skills;


[Pool(typeof(TheEngineerCardPool))]
public sealed class PerimeterWall() : TheEngineerCard(
    3,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_CARDS = 2m;
    private const decimal UPGRADE_CARDS = 1m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar((int)BASE_CARDS)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        IEnumerable<CardModel> blockCards = Owner.Character.CardPool
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.GainsBlock);

        List<CardModel> cards = CardFactory.GetDistinctForCombat(
                Owner,
                blockCards,
                (int)DynamicVars.Cards.BaseValue,
                Owner.RunState.Rng.CombatCardGeneration)
            .ToList();

        foreach (CardModel card in cards)
        {
            card.SetToFreeThisTurn();

            CardCmd.ApplyKeyword(
                card,
                CardKeyword.Ethereal);

            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_CARDS);
    }
}