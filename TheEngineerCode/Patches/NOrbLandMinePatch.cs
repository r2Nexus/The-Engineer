using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Orbs;
using TheEngineer.TheEngineerCode.Orbs;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(NOrb), nameof(NOrb.UpdateVisuals))]
public static class NOrbLandMinePatch
{
    private static readonly AccessTools.FieldRef<NOrb, object> PassiveLabelRef =
        AccessTools.FieldRefAccess<NOrb, object>("_passiveLabel");

    private static readonly AccessTools.FieldRef<NOrb, object> EvokeLabelRef =
        AccessTools.FieldRefAccess<NOrb, object>("_evokeLabel");

    private static readonly AccessTools.FieldRef<NOrb, object> LabelContainerRef =
        AccessTools.FieldRefAccess<NOrb, object>("_labelContainer");

    [HarmonyPostfix]
    public static void Postfix(NOrb __instance)
    {
        if (__instance.Model is not LandMineOrb)
            return;

        dynamic passiveLabel = PassiveLabelRef(__instance);
        dynamic evokeLabel = EvokeLabelRef(__instance);
        dynamic labelContainer = LabelContainerRef(__instance);

        if (passiveLabel == null || evokeLabel == null)
            return;

        if (labelContainer != null)
            labelContainer.Visible = true;

        passiveLabel.Visible = true;
        evokeLabel.Visible = true;

        passiveLabel.SetTextAutoSize(__instance.Model.PassiveVal.ToString("0"));
        evokeLabel.SetTextAutoSize(__instance.Model.EvokeVal.ToString("0"));
    }
}