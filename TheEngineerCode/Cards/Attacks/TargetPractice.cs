using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class TargetPractice() : TheEngineerCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 7m;
    private const decimal UPGRADE_DAMAGE = 3m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromOrb<TurretOrb>(),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-FIRE")
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE,ValueProp.Move),
        new ConsumeVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        bool consumed = await MaterialHelper.ConsumeMaterial(this, choiceContext, 1, MaterialSource.Hand, play);
        if(consumed) await TurretHelper.FireAllTurrets(choiceContext,Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}