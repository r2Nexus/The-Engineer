using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class RainingBullets() : TheEngineerCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies)
{
    private const decimal BASE_DAMAGE = 5m;
    private const decimal UPGRADE_DAMAGE = 3m;

    private const int FIRE_COUNT = 2;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-FIRE"),
        HoverTipFactory.FromOrb<TurretOrb>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play)
            .WithAttackerAnim(
                "Cast",
                Owner.Character.CastAnimDelay)
            .WithHitFx(
                "vfx/vfx_attack_blunt",
                tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);

        for (int i = 0; i < FIRE_COUNT; i++)
        {
            await TurretHelper.FireAllTurrets(
                choiceContext,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}