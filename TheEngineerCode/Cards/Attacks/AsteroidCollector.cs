using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class AsteroidCollector() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 7m;
    private const decimal UPGRADE_DAMAGE = 3m;

    private const decimal PRODUCE_PER_ATTACK = 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new ProduceVar(PRODUCE_PER_ATTACK)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);

        await CommonActions.ApplySelf<AsteroidCollectorPower>(
            this,
            DynamicVars.Produce().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}