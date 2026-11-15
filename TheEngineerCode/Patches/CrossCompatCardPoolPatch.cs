using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch]
public class CrossCompatCardPoolPatch
{
    [HarmonyPatch(typeof(CardPoolModel), "AllCards", MethodType.Getter)]
    public static class EngineerPoolPostfix
    {
        [HarmonyPostfix]
        public static void EngineerCardPoolPostfix(
            ref IEnumerable<CardModel> __result,
            CardPoolModel __instance)
        {
            if (__instance is not TokenCardPool &&
                __instance is not EventCardPool)
                return;

            if (AncientsAwakenedInterop.PerfectedCardPool(null) != null)
                return;

            __result = __result.Where(c =>
                c is not IAncientAwakenedCard
            ).ToArray();
        }
    }
}