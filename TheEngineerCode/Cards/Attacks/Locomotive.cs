using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Locomotive : TheEngineerCard
{
    private const string INCREASE_KEY = "Increase";
    private const string INCREASE_PER_WAGON_KEY = "IncreasePerWagon";

    private const int BASE_DAMAGE = 12;
    private const int UPGRADE_DAMAGE = 6;

    private const decimal BASE_INCREASE_PER_WAGON = 2m;
    private const decimal UPGRADE_INCREASE_PER_WAGON = 0m;

    private int _currentDamage = BASE_DAMAGE;
    private int _increasedDamage;

    public Locomotive() : base(
        2,
        CardType.Attack,
        CardRarity.Rare,
        TargetType.AnyEnemy)
    {
    }

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();

            _currentDamage = value;
            DynamicVars.Damage.BaseValue = value;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Wagon)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),

        new IntVar(
            INCREASE_PER_WAGON_KEY,
            BASE_INCREASE_PER_WAGON),
        
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),

        new CalculatedVar(INCREASE_KEY)
            .WithMultiplier((card, _) =>
                CountWagonsInStock(card)
                * card.DynamicVars[INCREASE_PER_WAGON_KEY].BaseValue)
    ];

    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        int damageIncrease = decimal.ToInt32(
            ((CalculatedVar)DynamicVars[INCREASE_KEY])
            .Calculate(play.Target));

        await CommonActions.CardAttack(this, play)
            .WithAttackerAnim(
                "Cast",
                Owner.Character.CastAnimDelay)
            .Execute(choiceContext);

        BuffFromPlay(damageIncrease);

        if (DeckVersion is Locomotive deckVersion)
            deckVersion.BuffFromPlay(damageIncrease);
    }

    protected override void OnUpgrade()
    {
        //DynamicVars[INCREASE_PER_WAGON_KEY].UpgradeValueBy(UPGRADE_INCREASE_PER_WAGON);
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }

    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromPlay(int extraDamage)
    {
        if (extraDamage <= 0)
            return;

        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = BASE_DAMAGE + IncreasedDamage;
    }

    private static decimal CountWagonsInStock(CardModel card)
    {
        return CountWagons(card, PileType.Hand)
             + CountWagons(card, PileType.Draw)
             + CountWagons(card, PileType.Discard);
    }

    private static int CountWagons(
        CardModel card,
        PileType pileType)
    {
        return pileType
            .GetPile(card.Owner)
            .Cards
            .Count(IsWagon);
    }

    private static bool IsWagon(CardModel card)
    {
        return card.Tags.Contains(TheEngineerCardTags.Wagon);
    }
}