using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Orbs;
using TheEngineer.TheEngineerCode.Orbs.Vfx;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(NOrb), nameof(NOrb.UpdateVisuals))]
public static class EngineerOrbVisualPatch
{
    [HarmonyPrefix]
    public static void Prefix(NOrb __instance)
    {
        if (!__instance.IsNodeReady())
            return;

        if (!CombatManager.Instance.IsInProgress)
            return;

        if (__instance.Model == null)
            return;

        // Only touch Engineer orbs.
        if (!__instance.Model.Id.Entry.StartsWith("THEENGINEER-"))
            return;

        // Vanilla only performs its sprite creation block
        // when this is null.
        if (__instance._sprite != null)
            return;

        Node2D sprite = __instance.Model.CreateSprite();

        if (sprite is not NEngineerOrbVfx orbVfx)
        {
            GD.PushWarning(
                $"Engineer orb {__instance.Model.Id.Entry} " +
                $"created unexpected visual type {sprite.GetType().FullName}.");

            sprite.QueueFree();
            return;
        }

        /*
         * Do this BEFORE AddChild.
         *
         * InitializeForOrb has its own guard, so this is safe
         * even if CreateSprite() already initialized the VFX.
         */
        orbVfx.InitializeForOrb(__instance.Model);

        __instance._sprite = sprite;
        __instance._orbVfx = orbVfx;

        __instance._visualContainer.AddChildSafely(sprite);

        sprite.Position = Vector2.Zero;

        __instance._curTween?.Kill();

        __instance._curTween =
            __instance.CreateTween();

        __instance._curTween
            .TweenProperty(
                sprite,
                "scale",
                Vector2.One,
                0.5)
            .From(Vector2.Zero)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }
}