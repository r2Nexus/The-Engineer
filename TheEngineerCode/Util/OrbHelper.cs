using System;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Util;

public static class OrbCmdHelper
{
    private static readonly MethodInfo EvokeSpecificMethod =
        typeof(OrbCmd).GetMethod(
            "Evoke",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            types: new[]
            {
                typeof(PlayerChoiceContext),
                typeof(Player),
                typeof(OrbModel),
                typeof(bool)
            },
            modifiers: null)
        ?? throw new MissingMethodException(
            "Could not find private OrbCmd.Evoke(PlayerChoiceContext, Player, OrbModel, bool).");

    public static async Task EvokeSpecific(
        PlayerChoiceContext choiceContext,
        Player player,
        OrbModel orb,
        bool dequeue = true)
    {
        object? result = EvokeSpecificMethod.Invoke(
            null,
            new object[] { choiceContext, player, orb, dequeue });

        if (result is not Task task)
            throw new InvalidOperationException("OrbCmd.Evoke did not return a Task.");

        await task;
    }
}