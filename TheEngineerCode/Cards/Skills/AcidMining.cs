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
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class AcidMining() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_STRENGTH_LOSS = 1m;
    private const decimal UPGRADE_STRENGTH_LOSS = 1m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TemporaryStrengthPower>(),
        HoverTipFactory.FromOrb<MinerOrb>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromCard<Material>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AcidMiningPower>(BASE_STRENGTH_LOSS)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await OrbCmd.Channel<MinerOrb>(choiceContext, Owner);

        int minerCount = TurretHelper.GetOrbs<MinerOrb>(Owner).Count;

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        
        if (minerCount <= 0)
            return;

        decimal strengthLoss =
            DynamicVars.Power<AcidMiningPower>().BaseValue * minerCount;

        if (strengthLoss <= 0m)
            return;

        await CommonActions.Apply<AcidMiningPower>(
            play.Target,
            this,
            -strengthLoss);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<AcidMiningPower>()
            .UpgradeValueBy(UPGRADE_STRENGTH_LOSS);
    }
}