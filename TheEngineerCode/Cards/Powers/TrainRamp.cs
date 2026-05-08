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
public class TrainRamp() : TheEngineerCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{

    private const decimal BASE_DAMAGE = 3;
    private const decimal UPGRADE_DAMAGE = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TrainRampPower>(BASE_DAMAGE)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<TrainRampPower>(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<TrainRampPower>().UpgradeValueBy(UPGRADE_DAMAGE);
    }
}