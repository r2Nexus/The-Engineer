using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class TeslaTurret() : TheEngineerCard(
    0,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy), IOnChargeSpent
{
    private const decimal BASE_DAMAGE = 5m;
    private const decimal UPGRADE_DAMAGE = 2m;

    private const decimal BASE_DAMAGE_GAIN = 2m;
    private const decimal UPGRADE_DAMAGE_GAIN = 1m;

    private decimal DamageGain =>
        BASE_DAMAGE_GAIN + (IsUpgraded ? UPGRADE_DAMAGE_GAIN : 0m);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CHARGE_MAX")
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    public async Task OnChargeSpent(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel? spentCard,
        decimal amount,
        AbstractModel? causedBy)
    {
        if (player != Owner)
            return;

        if (amount <= 0m)
            return;

        DynamicVars.Damage.BaseValue += DamageGain;

        if (!PileType.Hand.GetPile(player).Cards.Contains(this))
            await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}