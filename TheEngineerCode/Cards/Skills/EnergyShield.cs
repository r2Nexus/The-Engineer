using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class EnergyShield() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 8m;

    private const decimal BASE_CHARGE_INITIAL = 3m;

    private const decimal BASE_CHARGE_MAX = 8m;
    private const decimal UPGRADE_CHARGE_MAX = -2m;

    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Charge];
    protected override bool ShouldGlowGoldInternal => ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            ValueProp.Unpowered,
            null);

        if (await ChargeHelper.TrySpendFullCharge(choiceContext, this, this))
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ChargeMax().UpgradeValueBy(UPGRADE_CHARGE_MAX);
    }
}