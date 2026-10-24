using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public class LightOil() : TheEngineerCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_LIGHT = 1;
    private const decimal UPGRADE_LIGHT = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LightOilPower>(BASE_LIGHT)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<LightOilPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<LightOilPower>().UpgradeValueBy(UPGRADE_LIGHT);
    }
}