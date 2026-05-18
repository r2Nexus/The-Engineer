using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Powers;

public class TrainRampPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByHandDraw)
    {
        if(card.Owner.Creature != Owner) return;
        if (!card.Tags.Contains(TheEngineerCardTags.Wagon))
            return;

        Creature? target = CombatState.RunState.Rng.CombatTargets.NextItem(
            CombatState.HittableEnemies);

        if (target == null)
            return;

        Flash();

        
        await CreatureCmd.Damage(
            choiceContext,
            target,
            Amount,
            ValueProp.Unpowered,
            Owner,
            null);
    }
}