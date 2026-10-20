using BaseLib.Utils.ModInterop;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Util;

// Experimental Serum
[ModInterop(
    "AncientsAwakened",
    "AncientsAwakened.AncientsAwakenedCode.Extensions.CustomExperimentalCardExtension")]
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
[ModInterop(
    "AncientsAwakened",
    "AncientsAwakened.AncientsAwakenedCode.Extensions.CustomPerfectedCardExtension")]
public static class AncientsAwakenedPerfectedInterop
{
    public static void AddPerfectedCardForCustomCharacters(
        CardModel cardModel,
        CharacterModel characterModel)
    {
        // Sands Deltarune
    }
}