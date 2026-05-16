using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Util;

public static class DeferredOrbChannel
{
    private static int _channelDepth;
    private static bool _flushing;

    private static readonly Queue<Func<Task>> Queue = new();

    public static bool IsChannelResolving => _channelDepth > 0;

    public static void Enqueue<TOrb>(
        PlayerChoiceContext choiceContext,
        Player player)
        where TOrb : OrbModel
    {
        Queue.Enqueue(() => OrbCmd.Channel<TOrb>(choiceContext, player));
    }

    public static void EnterChannel()
    {
        _channelDepth++;
    }

    public static async Task ExitChannel(Task originalTask)
    {
        try
        {
            await originalTask;
        }
        finally
        {
            _channelDepth = Math.Max(0, _channelDepth - 1);

            if (_channelDepth == 0)
                await Flush();
        }
    }

    private static async Task Flush()
    {
        if (_flushing || CombatManager.Instance.IsOverOrEnding)
            return;

        _flushing = true;

        try
        {
            int safety = 0;

            while (Queue.Count > 0 && safety++ < 50)
                await Queue.Dequeue()();
        }
        finally
        {
            _flushing = false;
        }
    }
}