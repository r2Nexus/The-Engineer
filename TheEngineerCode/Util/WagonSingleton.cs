using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Util;

public sealed class WagonSingleton()
    : CustomSingletonModel(HookType.Combat)
{
    private sealed record WagonMemory(
        CardModel Card,
        Creature? Target);

    private readonly Dictionary<Player, WagonMemory> _lastWagons = [];

    private CardModel? _activeGhost;
    private bool _replayingGhost;

    public override Task BeforeCombatStart()
    {
        _lastWagons.Clear();
        _activeGhost = null;
        _replayingGhost = false;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (_replayingGhost)
            return;

        if (!card.Keywords.Contains(TheEngineerKeyWords.Wagon))
            return;

        Player owner = card.Owner;

        ICombatState? combatState =
            card.CombatState ?? owner.Creature.CombatState;

        if (combatState == null)
            return;

        _lastWagons.TryGetValue(
            owner,
            out WagonMemory? previous);

        // Current Wagon becomes the one remembered for next time.
        _lastWagons[owner] = new WagonMemory(
            card,
            cardPlay.Target);

        if (previous == null)
            return;

        CardModel ghost =
            combatState.CloneCard(previous.Card);

        Creature? target = ResolveReplayTarget(
            combatState,
            ghost,
            previous.Target);

        _activeGhost = ghost;
        _replayingGhost = true;

        try
        {
            await CardCmd.AutoPlay(
                choiceContext,
                ghost,
                target);
        }
        finally
        {
            _activeGhost = null;
            _replayingGhost = false;

            if (ghost.Pile is { IsCombatPile: true })
            {
                await CardPileCmd.RemoveFromCombat(
                    ghost,
                    skipVisuals: true);
            }
        }
    }

    private static Creature? ResolveReplayTarget(
        ICombatState combatState,
        CardModel ghost,
        Creature? previousTarget)
    {
        if (ghost.TargetType != TargetType.AnyEnemy)
            return previousTarget;

        if (previousTarget != null &&
            combatState.HittableEnemies.Contains(previousTarget))
        {
            return previousTarget;
        }

        return null;
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        bool isGhostDamage =
            _activeGhost != null &&
            (ReferenceEquals(cardSource, _activeGhost) ||
             ReferenceEquals(cardPlay?.Card, _activeGhost));

        return isGhostDamage ? 0m : 1m;
    }
}