using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

public class PiercingRounds() : TheEngineerCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    
    private const decimal BASE_DAMAGE = 8m;
    private const decimal UPGRADE_DAMAGE = 3m;
    
    private const decimal BASE_VULNERABLE = 2m;
    private const decimal UPGRADE_VULNERABLE = 0m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ConsumeVar(1),
        new PowerVar<VulnerablePower>(BASE_VULNERABLE),
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);
        
        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand);

        if (consumed)
        {
            if (play.Target != null)
                await CommonActions.Apply<VulnerablePower>(play.Target, this, DynamicVars.Power<VulnerablePower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
        //DynamicVars.Power<VulnerablePower>().UpgradeValueBy(UPGRADE_VULNERABLE);
    }
}