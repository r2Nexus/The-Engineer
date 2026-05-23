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
using TheEngineer.TheEngineerCode.Cards.Attacks;
using TheEngineer.TheEngineerCode.Cards.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.HoverTips;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Laboratory() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const int BASE_DRAW = 2;

    private const decimal BASE_CHARGE_INITIAL = 2m;
    private const decimal BASE_CHARGE_MAX = 6m;

    private const decimal BASE_SCIENCE_CARDS = 1m;
    private const decimal UPGRADE_SCIENCE_CARDS = 1m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Science)
    ];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];
    protected override bool ShouldGlowGoldInternal => ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar((int)BASE_SCIENCE_CARDS),
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(
            choiceContext,
            BASE_DRAW, Owner);

        if (await ChargeHelper.TrySpendFullCharge(choiceContext, this, this))
        {
            IEnumerable<CardModel> scienceCards = Owner.Character.CardPool
                .GetUnlockedCards(
                    Owner.UnlockState,
                    Owner.RunState.CardMultiplayerConstraint)
                .Where(card => card.Tags.Contains(TheEngineerCardTags.Science));

            List<CardModel> cards = CardFactory.GetDistinctForCombat(
                    Owner,
                    scienceCards,
                    (int)DynamicVars.Cards.BaseValue,
                    Owner.RunState.Rng.CombatCardGeneration)
                .ToList();

            foreach (CardModel card in cards)
            {
                card.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(
                    card,
                    PileType.Hand,
                    Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_SCIENCE_CARDS);
    }
}