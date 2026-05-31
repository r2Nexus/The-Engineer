using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class NuclearOption() : TheEngineerCard(4,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 30m;
    private const decimal DAMAGE_STACK = 8m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK")
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(BASE_DAMAGE),
        new ExtraDamageVar(DAMAGE_STACK),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) =>
                MaterialHelper.CountMaterial(card.Owner, MaterialSource.Stock))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        decimal damage = DynamicVars.CalculatedDamage.Calculate(play.Target);

        int consumed = await MaterialHelper.ConsumeAllMaterial(
            this,
            choiceContext,
            MaterialSource.Stock,
            play);

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(play.Target)
            .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}