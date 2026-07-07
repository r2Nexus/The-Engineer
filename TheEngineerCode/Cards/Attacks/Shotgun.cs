using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;


[Pool(typeof(TheEngineerCardPool))]
public class Shotgun() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies)
{
    private const decimal BASE_DAMAGE = 7m;
    private const decimal UPGRADE_DAMAGE = 2m;

    private const decimal BASE_WEAK = 1m;
    private const decimal UPGRADE_WEAK = 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new PowerVar<WeakPower>(BASE_WEAK),
        new ConsumeVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand,
            play);

        if (consumed)
        {
            foreach (Creature enemy in CombatState.HittableEnemies)
            {
                await CommonActions.Apply<WeakPower>(
                    enemy,
                    this,
                    DynamicVars.Power<WeakPower>().BaseValue);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
        DynamicVars.Power<WeakPower>().UpgradeValueBy(UPGRADE_WEAK);
    }
}