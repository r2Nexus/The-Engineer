using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheEngineer.TheEngineerCode.Ui;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter.OnHovered))]
public static class NEnergyCounterMaterialHoverPatch
{
    public static bool Prefix(NEnergyCounter __instance)
    {
        if (!GodotObject.IsInstanceValid(__instance))
            return true;

        EngineerMaterialCounter? materialCounter =
            __instance.GetNodeOrNull<EngineerMaterialCounter>(nameof(EngineerMaterialCounter))
            ?? __instance.FindChild(nameof(EngineerMaterialCounter), true, false) as EngineerMaterialCounter;

        if (materialCounter == null)
            return true;

        // If the mouse is over the Material arms, do not let
        // NEnergyCounter spawn its own Energy tooltip.
        return !materialCounter.IsMouseOverMaterialCounter();
    }
}