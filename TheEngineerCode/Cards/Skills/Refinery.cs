using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class Refinery() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const int OIL_PER_BLOCK = 3;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<Refinery>(
            "BlockAmount",
            static card => GetExpectedBlock(card).ToString())
    ];

    private static int GetExpectedBlock(Refinery card)
    {
        if (card.CombatState == null)
            return 0;

        int totalOil = 0;

        foreach (Creature enemy in card.CombatState.HittableEnemies)
        {
            totalOil += enemy.GetPowerAmount<OilPower>();
        }

        return totalOil / OIL_PER_BLOCK;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        int totalOilSpent = 0;
        
        if (CombatState != null)
            foreach (Creature enemy in CombatState.HittableEnemies.ToList())
            {
                OilPower? oil = enemy.GetPower<OilPower>();

                if (oil == null)
                    continue;

                totalOilSpent += await oil.TriggerOil(
                    choiceContext,
                    Owner.Creature,
                    this);
            }

        int block = totalOilSpent / OIL_PER_BLOCK;

        if (block > 0)
        {
            await CreatureCmd.GainBlock(
                Owner.Creature,
                block,
                ValueProp.Move,
                play);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}