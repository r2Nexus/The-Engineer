using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class PurpleSciencePower : TheEngineerPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStartLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player.Creature != Owner)
            return;

        if (Amount <= 0)
            return;

        for (int i = 0; i < Amount; i++)
        {
            CardModel? card = PickRandomUpgradableCard(player);

            if (card == null)
                break;

            Flash();
            CardCmd.Upgrade(card);
        }

        await Task.CompletedTask;
    }

    private static CardModel? PickRandomUpgradableCard(Player player)
    {
        List<CardModel> candidates = PileType.Hand
            .GetPile(player)
            .Cards
            .Where(card => card.IsUpgradable)
            .ToList();

        if (candidates.Count <= 0)
            return null;

        int index = player.RunState.Rng.CombatCardGeneration
            .NextInt(candidates.Count);

        return candidates[index];
    }
}