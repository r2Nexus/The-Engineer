using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class Tank : TheEngineerCard
{
    private const decimal BASE_BLOCK = 3m;
    private const decimal UPGRADE_BLOCK = 0m;

    private const decimal BASE_DAMAGE = 9m;
    private const decimal UPGRADE_DAMAGE = 3m;

    private const decimal BASE_CONSUME = 2m;
    private const decimal UPGRADE_CONSUME = 0m;

    private const int BASE_REPLAY = 2;

    public Tank() : base(
        2,
        CardType.Skill,
        CardRarity.Uncommon,
        TargetType.AnyEnemy)
    {
        _baseReplayCount = BASE_REPLAY;
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK")
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new ConsumeVar(BASE_CONSUME)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            ValueProp.Move,
            null);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Stock,
            play);

        if (!consumed)
            return;

        await CommonActions.CardAttack(this, play.Target)
            .WithAttackerAnim("attack", Owner.Character.AttackAnimDelay)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        //DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
        //DynamicVars.Consume().UpgradeValueBy(UPGRADE_CONSUME);
    }
}