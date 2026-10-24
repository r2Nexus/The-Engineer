using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TokenCardPool))]
public class BlueScience() : TheEngineerCard(1,
    CardType.Power, CardRarity.Token,
    TargetType.Self)
{
    private const decimal BASE_POWER = 1;
    private const decimal UPGRADE_POWER = 1;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Science];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FocusPower>(),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<FocusPower>(BASE_POWER)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        TheEngineerKeyWords.Material,
        TheEngineerKeyWords.Science
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<FocusPower>(this,DynamicVars.Power<FocusPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<FocusPower>().UpgradeValueBy(UPGRADE_POWER);
    }
}