using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class OilSpill() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    private const decimal BASE_OIL = 4m;
    private const decimal UPGRADE_OIL = 2m;

    private const decimal BASE_WEAKEN_ON_SPENT = 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OilPower>(BASE_OIL)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<OilPower>(
                enemy,
                this,
                DynamicVars.Power<OilPower>().BaseValue);

            await CommonActions.Apply<OilSpillPower>(
                enemy,
                this,
                BASE_WEAKEN_ON_SPENT);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OilPower>().UpgradeValueBy(UPGRADE_OIL);
    }
}