using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;


public class Pylon() : TheEngineerCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 13m;
    private const decimal UPGRADE_DAMAGE = 3m;

    private const decimal BASE_FOCUS = 4m;
    private const decimal UPGRADE_FOCUS = 1m;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new PowerVar<FocusedStrikePower>(BASE_FOCUS)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FocusedStrikePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);

        await CommonActions.ApplySelf<FocusedStrikePower>(
            this,
            DynamicVars.Power<FocusedStrikePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
        DynamicVars.Power<FocusedStrikePower>()
            .UpgradeValueBy(UPGRADE_FOCUS);
    }
}