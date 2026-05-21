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
public class MissileBarrage() : TheEngineerCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    
    private const decimal BASE_DAMAGE = 4m;
    private const decimal UPGRADE_DAMAGE = 1m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE,ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var consumed = await MaterialHelper.ConsumeAllMaterial(this, choiceContext, MaterialSource.Stock, play);
        if (consumed <= 0) return;
        
        for (var i = 0; i < consumed; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState)
                .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}