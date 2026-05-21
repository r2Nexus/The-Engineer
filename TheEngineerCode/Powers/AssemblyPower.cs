using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class AssemblyPower : TheEngineerPower, IOnConsumed
{
    private CardPlay? _queuedPlay;
    private CardModel? _queuedCard;
    private Creature? _queuedTarget;

    private bool _isPlayingAssemblyCopy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public Task OnConsumed(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy,
        CardPlay? play)
    {
        if (player != Owner.Player)
            return Task.CompletedTask;

        if (Amount <= 0)
            return Task.CompletedTask;

        if (amount <= 0)
            return Task.CompletedTask;

        if (_isPlayingAssemblyCopy)
            return Task.CompletedTask;

        if (_queuedPlay != null)
            return Task.CompletedTask;

        if (play == null)
            return Task.CompletedTask;

        if (!play.IsFirstInSeries)
            return Task.CompletedTask;

        CardModel card = play.Card;

        if (card.Owner != player)
            return Task.CompletedTask;

        _queuedPlay = play;
        _queuedCard = card;
        _queuedTarget = play.Target;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (_queuedPlay != play)
            return;

        CardModel? originalCard = _queuedCard;
        Creature? target = _queuedTarget;

        _queuedPlay = null;
        _queuedCard = null;
        _queuedTarget = null;

        if (originalCard == null)
            return;

        if (Amount <= 0)
            return;

        if (target != null && !target.IsHittable)
            target = null;

        Flash();

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -1,
            Owner,
            null);

        CardModel copy = originalCard.CreateDupe();
        
        copy.BaseReplayCount = 0;

        try
        {
            _isPlayingAssemblyCopy = true;

            await CardCmd.AutoPlay(
                choiceContext,
                copy,
                target);
        }
        finally
        {
            _isPlayingAssemblyCopy = false;
        }
    }
}