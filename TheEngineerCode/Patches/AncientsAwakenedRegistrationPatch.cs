using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards.Attacks;
using TheEngineer.TheEngineerCode.Cards.Skills;
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
        
        ModelDb.Card<ImprovisedDefend>()
            .AddPerfectedCardForCustomCharacters(
                ModelDb.Card<Defend>()
            );
        
        MainFile.Logger.Info(
            "[AncientsInterop] Registered Improvised Defend"
        );
        
        AncientsAwakenedExperimentalInterop
            .AddExperimentalCardForCustomCharacters(
                ModelDb.Card<IndustrialGrowth>(),
                ModelDb.Character<Character.TheEngineer>()
            );
        MainFile.Logger.Info(
            "[AncientsInterop] Registered Industrial Growth"
        );
    }
}