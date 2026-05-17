using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class TeslaTurret() : TheEngineerCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_DAMAGE = 12m;
    private const decimal UPGRADE_DAMAGE = 3m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CHARGE_MAX"),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),

        ..MakeCalculatedVar("CalculatedHits", 1,
            (card, target) =>
                0 + ChargeHelper.CountFullyChargedCards(card.Owner, card))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int hits = 1 + ChargeHelper.CountFullyChargedCards(Owner, this);

        for (int i = 0; i < hits; i++)
        {
            Creature? target = GetRandomEnemy();

            if (target == null)
                return;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3")
                .Execute(choiceContext);
        }
    }

    private Creature? GetRandomEnemy()
    {
        var enemies = CombatState.HittableEnemies;

        if (enemies.Count <= 0)
            return null;

        return enemies[Random.Shared.Next(enemies.Count)];
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}