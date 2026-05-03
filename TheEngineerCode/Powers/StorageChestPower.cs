using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class StorageChestPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(Amount)
    ];

    public override async Task BeforeFlushLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState, player))
            return;

        CardSelectorPrefs prefs = new CardSelectorPrefs(
            SelectionScreenPrompt,
            0,
            Amount);

        List<CardModel> selectedCards = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner.Player,
                prefs,
                RetainFilter,
                this))
            .ToList();

        if (selectedCards.Count == 0)
            return;

        foreach (CardModel card in selectedCards)
            card.GiveSingleTurnRetain();
    }

    private bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }
}