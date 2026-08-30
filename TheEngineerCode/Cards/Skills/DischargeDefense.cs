using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class DischargeDefense() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AnyAlly)
{
    private const decimal BASE_BLOCK = 5m;
    private const decimal UPGRADE_BLOCK = 3m;

    private const decimal BASE_CHARGE_INITIAL = 0m;
    private const decimal BASE_CHARGE_MAX = 5m;

    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override bool ShouldGlowGoldInternal =>
        ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move),

        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        Creature ally = play.Target;

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        await CreatureCmd.GainBlock(
            ally,
            DynamicVars.Block.BaseValue,
            ValueProp.Move,
            play);

        if (!await ChargeHelper.TrySpendFullCharge(
                choiceContext,
                this,
                this))
            return;

        // Read their Block AFTER giving them Block.
        decimal damage = ally.Block;

        if (damage <= 0)
            return;

        Creature? enemy = Owner.RunState.Rng.CombatTargets
            .NextItem(CombatState.HittableEnemies);

        if (enemy == null)
            return;

        await CommonActions.CardAttack(this,play.Target,damage,1).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}