using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Exoskeleton() : TheEngineerCard(
    2,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 12m;
    private const decimal UPGRADE_BLOCK = 4m;

    private const decimal BASE_DEXTERITY = 2m;
    private const decimal UPGRADE_DEXTERITY = 0m;

    private const decimal BASE_CHARGE_INITIAL = 3m;
    private const decimal UPGRADE_CHARGE_INITIAL = 0m;

    private const decimal BASE_CHARGE_MAX = 7m;
    private const decimal UPGRADE_CHARGE_MAX = 0m;
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Charge];
    protected override bool ShouldGlowGoldInternal => ChargeHelper.IsFull(this);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX),
        new PowerVar<DexterityPower>(BASE_DEXTERITY)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, DynamicVars.Block, play);

        if (await ChargeHelper.TrySpendFullCharge(choiceContext, this, this))
        {
            await CommonActions.ApplySelf<DexterityPower>(this, DynamicVars.Power<DexterityPower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
        //DynamicVars.Power<DexterityPower>().UpgradeValueBy(UPGRADE_DEXTERITY);
        DynamicVars.ChargeInitial().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeCurrent().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeMax().UpgradeValueBy(UPGRADE_CHARGE_MAX);
    }
}