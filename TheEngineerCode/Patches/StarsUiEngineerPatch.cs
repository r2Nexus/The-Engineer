using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(NStarCounter), nameof(NStarCounter.RefreshVisibility), new Type[] { })]
public class StarsUiEngineerPatch
{
    [HarmonyPostfix]
    public static void Postfix(NFoolCombatUi __instance)
    {
        if (__instance._player?.Character is not Character.TheEngineer)
            return;

        // stop re-application
        if (__instance.HasMeta("EngineerPositionAdjusted"))
            return;

        __instance.Position += new Vector2(0, 20);
        __instance.SetMeta("EngineerPositionAdjusted", true);

        MainFile.Logger.Info("Moved Stars UI.");
    }
}