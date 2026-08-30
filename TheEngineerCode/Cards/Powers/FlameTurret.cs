using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

public class FlameTurret() : TheEngineerCard(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_OIL = 3;
    private const decimal UPGRADE_OIL = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<FlameTurretPower>(BASE_OIL)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<FlameTurretPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {

    }
}