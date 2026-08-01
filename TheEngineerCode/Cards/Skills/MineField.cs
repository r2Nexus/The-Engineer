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

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class MineField() : TheEngineerCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_ORB_SLOTS = 2m;
    private const decimal UPGRADE_ORB_SLOTS = 1m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LandMineOrb>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MineFieldPower>(BASE_ORB_SLOTS)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        // This power must increase the capacity immediately
        // and remove the same number of slots at end of turn.
        await CommonActions.ApplySelf<MineFieldPower>(choiceContext,this);

        if (Owner.PlayerCombatState != null)
        {
            var orbQueue = Owner.PlayerCombatState.OrbQueue;

            int emptySlots = Math.Max(
                0,
                orbQueue.Capacity - orbQueue.Orbs.Count);

            for (int i = 0; i < emptySlots; i++)
            {
                await OrbCmd.Channel<LandMineOrb>(
                    choiceContext,
                    Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<MineFieldPower>().UpgradeValueBy(UPGRADE_ORB_SLOTS);
    }
}