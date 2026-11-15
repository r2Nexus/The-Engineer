using BaseLib.Utils.ModInterop;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Util;

// Experimental Serum
[ModInterop(
    "AncientsAwakened",
    "AncientsAwakened.AncientsAwakenedCode.CrossModCompatability.CustomExperimentalCardExtension")]
public static class AncientsAwakenedExperimentalInterop
{
    public static void AddExperimentalCardForCustomCharacters(
        CardModel cardModel,
        CharacterModel characterModel)
    {
        // Sans Undertale
    }
}

// Ancient Scepter
[ModInterop("AncientsAwakened", "AncientsAwakened.AncientsAwakenedCode.CrossModCompatability.CustomPerfectedCardExtension")]
public static class AncientsAwakenedInterop 
{
    public static void AddPerfectedCardForCustomCharacters(this CardModel cardModel, CardModel originalCardModel)
    {
        // Since this is a void, this method can be empty.
    }

    public static CardPoolModel? PerfectedCardPool(CardPoolModel? optionalFallBack)
    {
        return optionalFallBack; // Alternatively, you could call ModelDb.CardPool<MyCharacterPool>() directly and just pass null whenever this is called.
    }
}