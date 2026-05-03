using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;


[Pool(typeof(TheEngineerCardPool))]
public class CombatStim() : TheEngineerCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 7m;
    private const decimal UPGRADE_DAMAGE = 2m;
    
    private const decimal BASE_CHARGE_INITIAL = 0m;
    private const decimal UPGRADE_CHARGE_INITIAL = 6m;

    private const decimal BASE_CHARGE_MAX = 8m;
    private const decimal UPGRADE_CHARGE_MAX = 0m;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Charge];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX),
        
        new PowerVar<StrengthPower>(1),
        new PowerVar<FocusPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);
        
        if (ChargeHelper.TrySpendFullCharge(this))
        {
            await CommonActions.ApplySelf<StrengthPower>(this,DynamicVars.Power<StrengthPower>().BaseValue);
            await CommonActions.ApplySelf<FocusPower>(this, DynamicVars.Power<FocusPower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ChargeInitial().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeCurrent().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);

    }
}