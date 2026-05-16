using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(OrbCmd))]
public static class OrbCmdChannelPatch
{
    [HarmonyPatch(
        nameof(OrbCmd.Channel),
        typeof(PlayerChoiceContext),
        typeof(OrbModel),
        typeof(Player))]
    [HarmonyPrefix]
    private static void Prefix()
    {
        DeferredOrbChannel.EnterChannel();
    }

    [HarmonyPatch(
        nameof(OrbCmd.Channel),
        typeof(PlayerChoiceContext),
        typeof(OrbModel),
        typeof(Player))]
    [HarmonyPostfix]
    private static void Postfix(ref Task __result)
    {
        __result = DeferredOrbChannel.ExitChannel(__result);
    }
}