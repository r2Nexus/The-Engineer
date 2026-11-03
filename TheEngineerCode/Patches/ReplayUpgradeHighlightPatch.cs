using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(
    typeof(CardModel),
    nameof(CardModel.GetDescriptionForPile), typeof(PileType), typeof(CardModel.DescriptionPreviewType), typeof(Creature))]
public static class ReplayUpgradeHighlightPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        CardModel __instance,
        CardModel.DescriptionPreviewType previewType,
        ref string __result)
    {
        if (previewType != CardModel.DescriptionPreviewType.Upgrade)
            return;

        if (__instance is not TheEngineerCard engineer ||
            !engineer.ReplayIncreasedOnUpgrade)
            return;

        int replayCount = __instance.GetEnchantedReplayCount();

        if (replayCount <= 0)
            return;

        var original = new LocString(
            "static_hover_tips",
            "REPLAY.extraText"
        );

        original.Add("Times", replayCount);

        var highlighted = new LocString(
            "static_hover_tips",
            "REPLAY.extraText"
        );

        highlighted.Add("Times", $"[green]{replayCount}[/green]");

        string originalText = original.GetFormattedText() ?? "";
        string highlightedText = highlighted.GetFormattedText() ?? "";

        if (string.IsNullOrEmpty(originalText))
            return;

        int index = __result.LastIndexOf(
            originalText,
            StringComparison.Ordinal
        );

        if (index < 0)
            return;

        __result = __result.Remove(index, originalText.Length)
            .Insert(index, highlightedText);
    }
}