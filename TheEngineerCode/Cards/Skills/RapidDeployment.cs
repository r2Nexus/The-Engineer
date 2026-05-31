using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class RapidDeployment() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_CHARGE_INITIAL = 2m;
    private const decimal BASE_CHARGE_MAX = 6m;

    private const int BASE_CONSUME = 2;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override bool ShouldGlowGoldInternal =>
        ChargeHelper.IsFull(this);

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded
            ? []
            : [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromOrb<TurretOrb>();

            if (!IsUpgraded)
                yield return HoverTipFactory.FromKeyword(CardKeyword.Exhaust);
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ConsumeVar(BASE_CONSUME),

        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<TurretOrb>(choiceContext, Owner);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand,
            play: play);

        if (consumed)
            await OrbCmd.Channel<TurretOrb>(choiceContext, Owner);

        if (await ChargeHelper.TrySpendFullCharge(choiceContext, this, this))
            await OrbCmd.Channel<TurretOrb>(choiceContext, Owner);
    }
}