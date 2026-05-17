using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Liquefy() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 7m;
    private const decimal UPGRADE_DAMAGE = 2m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>(),
        HoverTipFactory.FromPower<ResiduePower>()
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

        Creature target = play.Target;

        await CommonActions.CardAttack(this, target)
            .Execute(choiceContext);

        ResiduePower? residue = target.Powers
            .OfType<ResiduePower>()
            .FirstOrDefault();

        if (residue == null || residue.Amount <= 0)
            return;

        int amount = residue.Amount;

        await PowerCmd.ModifyAmount(
            choiceContext,
            residue,
            -amount,
            null,
            this);

        await CommonActions.Apply<OilPower>(
            target,
            this,
            amount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}