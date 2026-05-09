using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Cards.Skills;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public class Polymerisation() : TheEngineerCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 2;
    private const decimal UPGRADE_BLOCK = 1;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PolymerisationPower>(BASE_BLOCK)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<PolymerisationPower>(this,DynamicVars.Power<PolymerisationPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<PolymerisationPower>().UpgradeValueBy(UPGRADE_BLOCK);
    }
}