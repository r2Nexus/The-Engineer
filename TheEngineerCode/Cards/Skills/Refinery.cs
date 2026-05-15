using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class Refinery() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL"),
        HoverTipFactory.FromPower<OilPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<OilPower>(BASE_OIL)
    ];

    private const decimal BASE_OIL = 6m;
    private const decimal UPGRADE_OIL = 1m;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int consumed = await MaterialHelper.ConsumeAllMaterial(this, choiceContext, MaterialSource.Hand);
        if (consumed > 0)
        {

            for (int i = 0; i < consumed; i++)
            {
                Creature? target = Owner.RunState.Rng.CombatTargets.NextItem(
                    CombatState.HittableEnemies);

                if (target == null)
                    return;
                
                await CommonActions.Apply<OilPower>(target, this,DynamicVars.Power<OilPower>().BaseValue);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OilPower>().UpgradeValueBy(UPGRADE_OIL);
    }
}