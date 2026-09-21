using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class Grenade() : TheEngineerCard(
    0,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    private const decimal BASE_DAMAGE = 4m;
    private const decimal UPGRADE_DAMAGE = 2m;

    private const decimal BASE_VULNERABLE = 1m;
    private const int BASE_CONSUME = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new PowerVar<VulnerablePower>(BASE_VULNERABLE),
        new ConsumeVar(BASE_CONSUME)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
            .WithHitFx(
                "vfx/vfx_attack_blunt",
                tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);

        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<VulnerablePower>(
                enemy,
                this,
                DynamicVars.Power<VulnerablePower>().BaseValue);
        }

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand,
            play);

        if (consumed)
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    CreateClone(),
                    PileType.Discard,
                    Owner));
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}