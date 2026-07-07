using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
public sealed class LaserTurret() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 4m;

    private const decimal BASE_CHARGE_INITIAL = 2m;
    private const decimal UPGRADE_CHARGE_INITIAL = 0m;
    
    private const decimal BASE_CHARGE_MAX = 8m;
    private const decimal UPGRADE_CHARGE_MAX = -2m;

    private const int CHARGED_EXTRA_HITS = 2;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromOrb<TurretOrb>()
    ];
    
    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];
    
    protected override bool ShouldGlowGoldInternal => ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),

        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX),

        ..MakeCalculatedVar("CalculatedHits", 0,
            (card, target) =>
                TurretHelper.GetOrbs<TurretOrb>(card.Owner).Count
                + (ChargeHelper.IsFull(card) ? CHARGED_EXTRA_HITS : 0))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        int hits = TurretHelper.GetOrbs<TurretOrb>(Owner).Count;

        if (await ChargeHelper.TrySpendFullCharge(choiceContext, this, this))
            hits += CHARGED_EXTRA_HITS;

        if (hits <= 0)
            return;

        for (int i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .Targeting(play.Target)
                .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ChargeInitial().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeCurrent().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeMax().UpgradeValueBy(UPGRADE_CHARGE_MAX);
    }
}