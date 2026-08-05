using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class Railgun() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private const decimal BASE_CHARGE = 0m;
    private const decimal BASE_MAX_CHARGE = 5m;

    private const decimal UPGRADE_CHARGE = 4m;
    private const decimal UPGRADE_MAX_CHARGE = 4m;

    protected override bool HasEnergyCostX => true;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override bool ShouldGlowGoldInternal =>
        ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChargeInitialVar(BASE_CHARGE),
        new ChargeCurrentVar(BASE_CHARGE),
        new ChargeMaxVar(BASE_MAX_CHARGE),

        new CalculationBaseVar(0m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) =>
                ChargeHelper.CountRemovableCharge(card.Owner, card))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        int energyMultiplier = ResolveEnergyXValue();

        // Prevent playing it for 0 energy from wasting all stored Charge.
        if (energyMultiplier <= 0)
            return;

        decimal chargeSpent =
            ChargeHelper.CountRemovableCharge(Owner, this);

        if (chargeSpent <= 0)
            return;

        decimal damagePerEnergy =
            DynamicVars.CalculatedDamage.Calculate(play.Target);

        decimal totalDamage =
            damagePerEnergy * energyMultiplier;

        await ChargeHelper.RemoveChargeFromAll(
            choiceContext,
            Owner,
            this,
            this);

        await DamageCmd.Attack(totalDamage)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithAttackerAnim(
                "Cast",
                Owner.Character.CastAnimDelay)
            .WithHitFx(
                "vfx/vfx_attack_blunt",
                tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ChargeInitial()
            .UpgradeValueBy(UPGRADE_CHARGE);

        DynamicVars.ChargeCurrent()
            .UpgradeValueBy(UPGRADE_CHARGE);

        DynamicVars.ChargeMax()
            .UpgradeValueBy(UPGRADE_MAX_CHARGE);
    }
}