using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards.Attacks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Patches;

[HarmonyPatch(typeof(ModelDb), "Init")]
public static class AncientsAwakenedRegistrationPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (AncientsAwakenedInterop.PerfectedCardPool(null) == null)
            return;

        ModelDb.Card<ImprovisedStrike>()
            .AddPerfectedCardForCustomCharacters(
                ModelDb.Card<Strike>()
            );

        MainFile.Logger.Info(
            "[AncientsInterop] Registered Improvised Strike"
        );
    }
}