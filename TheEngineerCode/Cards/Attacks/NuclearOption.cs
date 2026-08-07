using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class NuclearOption() : TheEngineerCard(
    BASE_COST,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy), IOnConsumed
{
    private const int BASE_COST = 16;

    private const decimal BASE_DAMAGE = 25m;
    private const decimal UPGRADE_DAMAGE = 5m;

    private int _consumedThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);
    }

    public Task OnConsumed(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy,
        CardPlay? play)
    {
        if (player != Owner || amount <= 0)
            return Task.CompletedTask;

        _consumedThisCombat = Math.Min(
            BASE_COST,
            _consumedThisCombat + amount);

        EnergyCost.SetCustomBaseCost(
            BASE_COST - _consumedThisCombat);

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}