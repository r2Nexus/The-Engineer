using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class ChainSignal() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_REPLAY = 1m;
    private const decimal UPGRADE_REPLAY = 1m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Wagon)
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ChainSignalPower>(BASE_REPLAY)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<ChainSignalPower>(
            this,
            DynamicVars.Power<ChainSignalPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<ChainSignalPower>()
            .UpgradeValueBy(UPGRADE_REPLAY);
    }
}