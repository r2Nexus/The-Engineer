using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class PersonalBattery() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const int BASE_CARDS = 2;

    private const decimal BASE_CHARGE_INITIAL = 2m;
    private const decimal BASE_CHARGE_MAX = 7m;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Charge
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(BASE_CARDS),

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

        bool charged = ChargeHelper.TrySpendFullCharge(this);

        PileType destination = charged
            ? PileType.Hand
            : PileType.Draw;

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            PowerCell powerCell = CombatState.CreateCard<PowerCell>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(powerCell);

            await CardPileCmd.AddGeneratedCardToCombat(
                powerCell,
                destination,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
    }
}