using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class LaserTurret() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 5m;

    private const int BASE_HITS = 2;
    private const int CHARGED_EXTRA_HITS = 2;

    private const decimal BASE_CHARGE_INITIAL = 2m;
    private const decimal UPGRADE_CHARGE_INITIAL = 0m;

    private const decimal BASE_CHARGE_MAX = 8m;
    private const decimal UPGRADE_CHARGE_MAX = -2m;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override bool ShouldGlowGoldInternal =>
        ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),

        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX),

        ..MakeCalculatedVar(
            "CalculatedHits",
            2,
            (card, _) =>
                ChargeHelper.IsFull(card)
                    ? CHARGED_EXTRA_HITS
                    : 0)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        int hitCount = (int)
            ((CustomCalculatedVar)DynamicVars["CalculatedHits"])
            .CalculateCustom(play.Target);

        bool wasFullyCharged = ChargeHelper.IsFull(this);

        await CommonActions.CardAttack(
                this,
                play,
                hitCount)
            .Execute(choiceContext);

        if (wasFullyCharged)
        {
            await ChargeHelper.TrySpendFullCharge(
                choiceContext,
                this,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ChargeInitial()
            .UpgradeValueBy(UPGRADE_CHARGE_INITIAL);

        DynamicVars.ChargeCurrent()
            .UpgradeValueBy(UPGRADE_CHARGE_INITIAL);

        DynamicVars.ChargeMax()
            .UpgradeValueBy(UPGRADE_CHARGE_MAX);
    }
}