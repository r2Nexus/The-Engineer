using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class PowerCell() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Token,
    TargetType.Self)
{
    private const decimal BASE_ENERGY = 1m;
    private const decimal BASE_FOCUS = 1m;
    private const decimal UPGRADE_FOCUS = 1m;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar((int)BASE_ENERGY),
        new PowerVar<PersonalBatteryPower>(BASE_FOCUS)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
        HoverTipFactory.FromPower<PersonalBatteryPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(
            DynamicVars.Energy.IntValue,
            Owner);

        await CommonActions.ApplySelf<PersonalBatteryPower>(
            this,
            DynamicVars.Power<PersonalBatteryPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<PersonalBatteryPower>().UpgradeValueBy(UPGRADE_FOCUS);
    }
}