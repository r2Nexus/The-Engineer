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
public class Biochamber() : TheEngineerCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    private const int BASE_OIL = 3;
    private const int UPGRADE_OIL = 1;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>(),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<BiochamberPower>(BASE_OIL)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<BiochamberPower>(this, DynamicVars.Power<BiochamberPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<BiochamberPower>().UpgradeValueBy(UPGRADE_OIL);
    }
}