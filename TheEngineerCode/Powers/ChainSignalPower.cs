using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;
public sealed class ChainSignalPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyCardPlayCount(
        CardModel card,
        Creature? target,
        int playCount)
    {
        if (Amount <= 0)
            return playCount;

        if (card.Owner.Creature != Owner)
            return playCount;

        if (!IsWagon(card))
            return playCount;

        return playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();

        // See note below.
        Amount--;

        return Task.CompletedTask;
    }

    private static bool IsWagon(CardModel card)
    {
        return card.Tags.Contains(TheEngineerCardTags.Wagon);
    }
}