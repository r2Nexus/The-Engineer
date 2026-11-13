using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheEngineer.TheEngineerCode.Cards;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(NFoolCombatUi), nameof(NFoolCombatUi.RefreshVisibility), new Type[] { })]
public static class FoolUiEngineerPatch
{
    [HarmonyPostfix]
    public static void Postfix(NFoolCombatUi __instance)
    {
        if (__instance._player?.Character is not Character.TheEngineer)
            return;

        // stop re-application
        if (__instance.HasMeta("EngineerPositionAdjusted"))
            return;

        __instance.Position += new Vector2(0, -75);
        __instance.SetMeta("EngineerPositionAdjusted", true);

        MainFile.Logger.Info("Moved Fool UI.");
    }
}