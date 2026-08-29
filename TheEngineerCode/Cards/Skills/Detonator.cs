using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Detonator() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    private const decimal BASE_OIL = 7m;
    private const decimal UPGRADE_OIL = 2m;

    private const decimal BASE_CHARGE_INITIAL = 0m;
    private const decimal BASE_CHARGE_MAX = 6m;

    private const int LAND_MINES = 2;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override bool ShouldGlowGoldInternal =>
        ChargeHelper.IsFull(this);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LandMineOrb>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OilPower>(BASE_OIL),

        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        await CommonActions.Apply<OilPower>(
            cardPlay.Target,
            this,
            DynamicVars.Power<OilPower>().BaseValue);

        if (await ChargeHelper.TrySpendFullCharge(
                choiceContext,
                this,
                this))
        {
            for (int i = 0; i < LAND_MINES; i++)
            {
                await OrbCmd.Channel<LandMineOrb>(
                    choiceContext,
                    Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OilPower>().UpgradeValueBy(UPGRADE_OIL);
    }
}