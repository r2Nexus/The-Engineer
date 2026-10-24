using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Ui;

namespace TheEngineer.TheEngineerCode.Util;

public static class EngineerCycleHoverTips
{
    private static IEnumerable<CardModel> GetCards()
    {
        return ModelDb.CardPool<TheEngineerCardPool>()
            .AllCards
            .Concat(ModelDb.CardPool<TokenCardPool>().AllCards)
            .OfType<TheEngineerCard>()
            .OrderBy(card => card.Rarity)
            .ThenBy(card => card.Title.ToString());
    }

    public static IHoverTip ForTag(
        CardTag tag,
        double secondsPerCard = 0.9)
    {
        return new CycleHoverTip(
            GetCards().Where(card => card.Tags.Contains(tag)).ToArray(),
            secondsPerCard: secondsPerCard);
    }

    public static IHoverTip ForKeyword(
        CardKeyword keyword,
        double secondsPerCard = 0.9)
    {
        return new CycleHoverTip(
            GetCards().Where(card => card.Keywords.Contains(keyword)).ToArray(),
            secondsPerCard: secondsPerCard);
    }
}