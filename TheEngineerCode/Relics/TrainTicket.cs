using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Relics;

public sealed class TrainTicket : TheEngineerRelic
{
    private bool _triggeredThisTurn;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerCycleHoverTips.ForTag(TheEngineerCardTags.Wagon)
    ];

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (_triggeredThisTurn)
            return;

        CardModel card = cardPlay.Card;
        
        if (card.Owner != Owner)
            return;

        if (!card.Tags.Contains(TheEngineerCardTags.Wagon))
            return;

        _triggeredThisTurn = true;

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            1,
            Owner);
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player == Owner)
            _triggeredThisTurn = false;

        return Task.CompletedTask;
    }
}