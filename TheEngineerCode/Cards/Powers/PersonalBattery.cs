using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class PersonalBattery() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_ENERGY = 1m;
    private const decimal BASE_FOCUS = 2m;

    private const decimal BASE_CHARGE_INITIAL = 3m;
    private const decimal UPGRADE_CHARGE_INITIAL = 3m;

    private const decimal BASE_CHARGE_MAX = 6m;
    private const decimal UPGRADE_CHARGE_MAX = 0m;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar((int)BASE_ENERGY),
        new PowerVar<PersonalBatteryPower>(BASE_FOCUS),
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(
            (int)DynamicVars.Energy.BaseValue,
            Owner);

        if (!ChargeHelper.TrySpendFullCharge(this))
            return;

        await CommonActions.ApplySelf<PersonalBatteryPower>(
            this,
            DynamicVars.Power<PersonalBatteryPower>().BaseValue);
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