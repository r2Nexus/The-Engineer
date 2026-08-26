using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class MissileBarrage() : TheEngineerCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.RandomEnemy)
{
    private const string CALCULATED_HITS_KEY = "CalculatedHits";

    private const decimal BASE_DAMAGE = 4m;
    private const decimal UPGRADE_DAMAGE = 1m;

    private const int BASE_HITS = 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),

        ..MakeCalculatedVar(
            CALCULATED_HITS_KEY,
            BASE_HITS,
            (card, _) =>
                MaterialHelper.CountMaterial(
                    card,
                    MaterialSource.Stock))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int consumed = await MaterialHelper.ConsumeAllMaterial(
            this,
            choiceContext,
            MaterialSource.Stock,
            play);

        int hits = BASE_HITS + consumed;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hits)
            .FromCard(this, play)
            .TargetingRandomOpponents(CombatState)
            .WithAttackerAnim(
                "Cast",
                Owner.Character.CastAnimDelay)
            .WithHitFx(
                "vfx/vfx_attack_blunt",
                tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}