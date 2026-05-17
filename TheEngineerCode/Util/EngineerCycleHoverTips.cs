using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.HoverTips;

namespace TheEngineer.TheEngineerCode.Util;

public static class EngineerCycleHoverTips
{
    private static readonly Dictionary<CardTag, CardModel[]> Cache = new();

    private static readonly MethodInfo ModelDbCardMethod =
        typeof(ModelDb)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == nameof(ModelDb.Card) &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters().Length == 0);

    public static IHoverTip ForTag(
        CardTag tag,
        double secondsPerCard = 0.9)
    {
        return new CycleHoverTip(
            GetCardsWithTag(tag),
            secondsPerCard: secondsPerCard);
    }

    public static IEnumerable<IHoverTip> ForCardTags(
        CardModel card,
        double secondsPerCard = 0.9)
    {
        if (card.Tags.Contains(TheEngineerCardTags.Science))
            yield return ForTag(TheEngineerCardTags.Science, secondsPerCard);

        if (card.Tags.Contains(TheEngineerCardTags.Wagon))
            yield return ForTag(TheEngineerCardTags.Wagon, secondsPerCard);

        if (card.Tags.Contains(TheEngineerCardTags.Charge))
            yield return ForTag(TheEngineerCardTags.Charge, secondsPerCard);
    }

    private static CardModel[] GetCardsWithTag(CardTag tag)
    {
        if (Cache.TryGetValue(tag, out CardModel[]? cached))
            return cached;

        CardModel[] cards = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(IsEngineerCardType)
            .Select(CreateModel)
            .Where(card => card.Tags.Contains(tag))
            .OrderBy(card => card.Rarity)
            .ThenBy(card => card.Title.ToString())
            .ToArray();

        Cache[tag] = cards;
        return cards;
    }

    private static bool IsEngineerCardType(Type type)
    {
        return !type.IsAbstract
            && type.IsClass
            && typeof(TheEngineerCard).IsAssignableFrom(type);
    }

    private static CardModel CreateModel(Type cardType)
    {
        return (CardModel)ModelDbCardMethod
            .MakeGenericMethod(cardType)
            .Invoke(null, null)!;
    }
}