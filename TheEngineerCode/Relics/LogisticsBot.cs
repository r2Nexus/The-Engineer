using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Relics;

public sealed class LogisticsBot : TheEngineerRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    public override Task BeforeFlush(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner)
            return Task.CompletedTask;

        List<CardModel> skills = PileType.Hand
            .GetPile(player)
            .Cards
            .Where(card =>
                card.Type == CardType.Skill &&
                !card.ShouldRetainThisTurn)
            .ToList();

        if (skills.Count == 0)
            return Task.CompletedTask;

        CardModel? selected =
            Owner.RunState.Rng.CombatCardSelection.NextItem(skills);

        selected.GiveSingleTurnRetain();

        Flash();

        return Task.CompletedTask;
    }
}