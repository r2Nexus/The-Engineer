using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class RailSignalPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Owner.Creature != Owner)
            return false;

        if (!card.Tags.Contains(TheEngineerCardTags.Wagon))
            return false;

        PileType? pileType = card.Pile?.Type;

        if (pileType != PileType.Hand &&
            pileType != PileType.Play)
            return false;

        modifiedCost = Math.Max(0m, originalCost - 1m);

        return modifiedCost != originalCost;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (card.Owner.Creature != Owner)
            return;

        if (!card.Tags.Contains(TheEngineerCardTags.Wagon))
            return;

        PileType? pileType = card.Pile?.Type;

        if (pileType != PileType.Hand &&
            pileType != PileType.Play)
            return;

        Flash();

        await PowerCmd.Decrement(this);
    }
}