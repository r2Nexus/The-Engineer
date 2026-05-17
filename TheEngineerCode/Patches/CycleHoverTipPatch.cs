using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TheEngineer.TheEngineerCode.HoverTips;

namespace TheEngineer.TheEngineerCode.Patches;

internal sealed class CycleHoverTipState
{
    public Control Owner = null!;
    public List<IHoverTip> OriginalTips = new();
    public HoverTipAlignment Alignment;
    public int LastVersion;
}

internal static class CycleHoverTipPatchStorage
{
    public static readonly Dictionary<NHoverTipSet, CycleHoverTipState> Active = new();
}

[HarmonyPatch(
    typeof(NHoverTipSet),
    nameof(NHoverTipSet.CreateAndShow),
    [
        typeof(Control),
        typeof(IEnumerable<IHoverTip>),
        typeof(HoverTipAlignment)
    ])]
public static class NHoverTipSetCreateAndShowCyclePatch
{
    private static void Prefix(
        ref IEnumerable<IHoverTip> hoverTips,
        out List<IHoverTip> __state)
    {
        __state = hoverTips.ToList();
        
        hoverTips = HoverTipResolver.ResolveAll(__state);
    }

    private static void Postfix(
        Control owner,
        HoverTipAlignment alignment,
        NHoverTipSet? __result,
        List<IHoverTip> __state)
    {
        if (__result == null)
            return;

        if (!HoverTipResolver.HasResolvingTip(__state))
            return;

        CycleHoverTipPatchStorage.Active[__result] = new CycleHoverTipState
        {
            Owner = owner,
            OriginalTips = __state,
            Alignment = alignment,
            LastVersion = HoverTipResolver.GetVersionKey(__state)
        };
    }
}

[HarmonyPatch(typeof(NHoverTipSet), nameof(NHoverTipSet._Process))]
public static class NHoverTipSetProcessCyclePatch
{
    private static void Postfix(NHoverTipSet __instance)
    {
        if (!CycleHoverTipPatchStorage.Active.TryGetValue(__instance, out CycleHoverTipState? state))
            return;

        int version = HoverTipResolver.GetVersionKey(state.OriginalTips);

        if (version == state.LastVersion)
            return;

        state.LastVersion = version;

        Rebuild(__instance, state);
    }

    private static void Rebuild(
        NHoverTipSet set,
        CycleHoverTipState state)
    {
        ClearChildren(set._textHoverTipContainer);
        ClearChildren(set._cardHoverTipContainer);

        set._textHoverTipContainer.Size = Vector2.Zero;
        set._cardHoverTipContainer.Size = Vector2.Zero;

        set.Init(state.Owner, HoverTipResolver.ResolveAll(state.OriginalTips));

        if (state.Alignment != HoverTipAlignment.None)
            set.SetAlignment(state.Owner, state.Alignment);
        else
        {
            set.CorrectVerticalOverflow();
            set.CorrectHorizontalOverflow();
        }
    }

    private static void ClearChildren(Node parent)
    {
        foreach (Node child in parent.GetChildren().OfType<Node>().ToList())
        {
            parent.RemoveChild(child);
            child.QueueFree();
        }
    }
}

[HarmonyPatch(typeof(NHoverTipSet), nameof(NHoverTipSet.Remove))]
public static class NHoverTipSetRemoveCyclePatch
{
    private static void Prefix(Control owner)
    {
        if (NHoverTipSet._activeHoverTips.TryGetValue(owner, out NHoverTipSet? set))
            CycleHoverTipPatchStorage.Active.Remove(set);
    }
}