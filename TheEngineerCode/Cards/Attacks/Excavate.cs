using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class Excavate() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Ancient,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 9m;
    private const decimal UPGRADE_DAMAGE = 3m;

    private const decimal BASE_PRODUCE = 2m;
    private const decimal BASE_FREE_CONSUME = 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new ProduceVar(BASE_PRODUCE),
        new PowerVar<FreeConsumePower>(BASE_FREE_CONSUME)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FreeConsumePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);

        await MaterialHelper.ProduceMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand);

        await CommonActions.ApplySelf<FreeConsumePower>(
            this,
            DynamicVars.Power<FreeConsumePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}