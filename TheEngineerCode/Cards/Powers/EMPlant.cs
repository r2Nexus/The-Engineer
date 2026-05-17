using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public class EMPlant() : TheEngineerCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CHARGE_MAX"),
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Charge)
    ];

    private const decimal BASE_CHARGE = 1;
    private const decimal UPGRADE_CHARGE = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<EMPlantPower>(BASE_CHARGE)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<EMPlantPower>(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<EMPlantPower>().UpgradeValueBy(UPGRADE_CHARGE);
    }
}