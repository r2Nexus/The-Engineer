using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class ResearchWagon() : TheEngineerCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 16m;
    private const decimal UPGRADE_DAMAGE = 4m;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Wagon
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Science),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CommonActions.CardAttack(this, play)
            .WithAttackerAnim(
                "Cast",
                Owner.Character.CastAnimDelay)
            .Execute(choiceContext);

        CardSelectorPrefs prefs = new(
            CardSelectorPrefs.TransformSelectionPrompt,
            1);

        CardModel? original = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                prefs,
                null,
                this))
            .FirstOrDefault();

        if (original == null)
            return;

        IEnumerable<CardModel> sciencePool = Owner.Character.CardPool
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card =>
                card.Tags.Contains(TheEngineerCardTags.Science));

        CardModel? science = CardFactory.GetDistinctForCombat(
                Owner,
                sciencePool,
                1,
                Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (science == null)
            return;

        var transformResult = await CardCmd.Transform(
            original,
            science);

        if (!transformResult.HasValue)
            return;

        CardModel transformedCard =
            transformResult.Value.cardAdded;

        transformedCard.EnergyCost.UpgradeBy(-1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(
            UPGRADE_DAMAGE);
    }
}