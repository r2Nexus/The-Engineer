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
public class ScientificForm() : TheEngineerCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    private const int BASE_SCIENCE = 2;
    private const int UPGRADE_SCIENCE = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ScientificFormPower>(BASE_SCIENCE)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForKeyword(TheEngineerKeyWords.Science)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<ScientificFormPower>(choiceContext,this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<ScientificFormPower>().UpgradeValueBy(UPGRADE_SCIENCE);
    }
}