using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class OilBarrel() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Token,
    TargetType.RandomEnemy)
{
    private const decimal BASE_OIL = 8m;
    private const decimal UPGRADE_OIL = 3m;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<OilPower>(BASE_OIL)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<OilPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Creature? target = Owner.RunState.Rng.CombatTargets.NextItem(
            CombatState.HittableEnemies);

        if (target == null)
            return;

        await CommonActions.Apply<OilPower>(target,this,DynamicVars.Power<OilPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OilPower>().UpgradeValueBy(UPGRADE_OIL);
    }
}