using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

public class RepairSystem() : TheEngineerCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_REPAIR = 3m;
    private const decimal UPGRADE_REPAIR = 1m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RepairSystemPower>(BASE_REPAIR)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<RepairSystemPower>(this, DynamicVars.Power<RepairSystemPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<RepairSystemPower>().UpgradeValueBy(UPGRADE_REPAIR);
    }
}