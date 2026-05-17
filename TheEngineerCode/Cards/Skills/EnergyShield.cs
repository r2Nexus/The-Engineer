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
    private const decimal BASE_BLOCK = 7m;
    private const decimal UPGRADE_BLOCK = 0m;

    private const decimal BASE_CHARGED_BLOCK = 8m;
    private const decimal UPGRADE_CHARGED_BLOCK = 0m;

    private const decimal BASE_CHARGE_INITIAL = 3m;
    private const decimal UPGRADE_CHARGE_INITIAL = 0m;

    private const decimal BASE_CHARGE_MAX = 8m;
    private const decimal UPGRADE_CHARGE_MAX = -2m;

    private const string CHARGED_BLOCK_VAR = "ChargedBlock";

    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Charge];
    protected override bool ShouldGlowGoldInternal => ChargeHelper.IsFull(this);

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX),
        new IntVar(CHARGED_BLOCK_VAR, BASE_CHARGED_BLOCK)
    };

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            ValueProp.Unpowered,
            null);

        if (ChargeHelper.TrySpendFullCharge(this))
        {
            await CreatureCmd.GainBlock(
                Owner.Creature,
                DynamicVars[CHARGED_BLOCK_VAR].BaseValue,
                ValueProp.Unpowered,
                null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
        DynamicVars[CHARGED_BLOCK_VAR].UpgradeValueBy(UPGRADE_CHARGED_BLOCK);
        DynamicVars.ChargeInitial().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeCurrent().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeMax().UpgradeValueBy(UPGRADE_CHARGE_MAX);
    }
}