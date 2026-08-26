using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;


[Pool(typeof(TheEngineerCardPool))]
public class GreenScience() : TheEngineerCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_POWER = 2;
    private const decimal UPGRADE_POWER = 1;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Science];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<GreenSciencePower>(BASE_POWER),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CHARGE_MAX"),
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Charge)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<GreenSciencePower>(this,DynamicVars.Power<GreenSciencePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<GreenSciencePower>().UpgradeValueBy(UPGRADE_POWER);
    }
}