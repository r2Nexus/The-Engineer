using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.HoverTips;

public static class CycleHoverTip
{
    public static IHoverTip FromCards(
        IReadOnlyList<CardModel> cards,
        double secondsPerCard = 1.25,
        bool upgrade = false)
    {
        if (cards == null || cards.Count == 0)
            throw new ArgumentException("Cycling card hover tip needs at least one card.", nameof(cards));

        CardModel card = PickCard(cards, secondsPerCard);
        return HoverTipFactory.FromCard(card, upgrade);
    }

    public static IEnumerable<IHoverTip> FromCardsWithCardHoverTips(
        IReadOnlyList<CardModel> cards,
        double secondsPerCard = 1.25,
        bool upgrade = false)
    {
        if (cards == null || cards.Count == 0)
            yield break;

        CardModel card = PickCard(cards, secondsPerCard);

        yield return HoverTipFactory.FromCard(card, upgrade);
        
        foreach (IHoverTip tip in card.HoverTips)
            yield return tip;
    }

    private static CardModel PickCard(
        IReadOnlyList<CardModel> cards,
        double secondsPerCard)
    {
        double safeSeconds = Math.Max(0.25, secondsPerCard);
        double elapsedSeconds = Time.GetTicksMsec() / 1000.0;

        int index = (int)(Math.Floor(elapsedSeconds / safeSeconds) % cards.Count);
        return cards[index];
    }
}