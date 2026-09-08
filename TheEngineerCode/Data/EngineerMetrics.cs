using System.Reflection;
using System.Text.Json;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Data;

public static class EngineerMetrics
{
    private const int MinimumFloors = 5;

    private static readonly HashSet<Assembly> AllowedAssemblies =
    [
        // Base game
        typeof(CharacterModel).Assembly,

        // The Engineer
        typeof(TheEngineerCardPool).Assembly
    ];

    public static void OnMetricsUpload(
        SerializableRun run,
        bool isVictory,
        ulong localPlayerId)
    {
        if (!ShouldUpload(run, localPlayerId))
            return;

        try
        {
            RunMetrics metrics;

            // Keeps any localized strings in the collected data consistent.
            LocManager.Instance.StartOverridingLanguageAsEnglish();

            try
            {
                metrics = GetRunMetrics(
                    run,
                    isVictory,
                    localPlayerId);
            }
            finally
            {
                LocManager.Instance.StopOverridingLanguageAsEnglish();
            }

            string dataJson = JsonSerializer.Serialize(
                metrics,
                MetricsSerializerContext.Default.RunMetrics);

            bool hasForeignContent =
                HasForeignContent(run);

            string wrappedJson =
                $$"""
                {
                    "schema_version": 1,
                    "mod_version": {{JsonSerializer.Serialize(GetModVersion())}},
                    "has_foreign_content": {{JsonSerializer.Serialize(hasForeignContent)}},
                    "data": {{dataJson}}
                }
                """;

            Console.WriteLine(
                $"[TheEngineer] Uploading run metrics. " +
                $"Foreign content: {hasForeignContent}");

            _ = EngineerMetricsClient.Upload(wrappedJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[TheEngineer] Failed to prepare metrics:\n{ex}");
        }
    }

    private static bool ShouldUpload(
        SerializableRun run,
        ulong localPlayerId)
    {
        // Respect the game's own metrics preference.
        if (!MetricUtilities.ShouldUploadMetrics())
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "game metrics are disabled.");

            return false;
        }

        // Engineer-specific opt-in.
        if (!EngineerConfig.UploadMetrics)
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "Engineer metrics are disabled.");

            return false;
        }

        // Don't treat deliberately abandoned runs as normal losses.
        if (RunManager.Instance.IsAbandoned)
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "run was abandoned.");

            return false;
        }

        // Keep the dataset focused on normal runs.
        if (run.GameMode != GameMode.Standard)
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "not a Standard run.");

            return false;
        }

        int floorCount =
            run.MapPointHistory
                .SelectMany(entries => entries)
                .Count();

        // Mostly filters runs that barely started.
        if (floorCount < MinimumFloors)
        {
            Console.WriteLine(
                $"[TheEngineer] Metrics skipped: " +
                $"only {floorCount} floors reached.");

            return false;
        }

        SerializablePlayer? localPlayer =
            run.Players.FirstOrDefault(
                player =>
                    (long)player.NetId ==
                    (long)localPlayerId);

        if (localPlayer?.CharacterId is not { } characterId)
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "could not find local player.");

            return false;
        }

        CharacterModel? character =
            ModelDb.GetByIdOrNull<CharacterModel>(
                characterId);

        if (character == null)
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "could not resolve character.");

            return false;
        }

        bool isEngineer =
            character.GetType().Assembly ==
            typeof(TheEngineerCardPool).Assembly;

        if (!isEngineer)
        {
            Console.WriteLine(
                "[TheEngineer] Metrics skipped: " +
                "local player is not The Engineer.");

            return false;
        }

        return true;
    }

    private static RunMetrics GetRunMetrics(
        SerializableRun run,
        bool isVictory,
        ulong localPlayerId)
    {
        ModelId killedBy = ModelId.none;

        MapPointHistoryEntry? finalEntry =
            run.MapPointHistory
                .LastOrDefault()?
                .LastOrDefault();

        if (!isVictory &&
            finalEntry != null &&
            finalEntry.Rooms.Count > 0 &&
            finalEntry.Rooms.Last().RoomType.IsCombatRoom())
        {
            killedBy =
                finalEntry.Rooms.Last().ModelId
                ?? ModelId.none;
        }

        SerializablePlayer localPlayer =
            run.Players.First(
                player =>
                    (long)player.NetId ==
                    (long)localPlayerId);

        List<MapPointHistoryEntry> history =
            run.MapPointHistory
                .SelectMany(entries => entries)
                .ToList();

        List<EncounterMetric> encounters =
            history
                .Where(entry =>
                    entry.Rooms.Count > 0 &&
                    entry.Rooms.Last().RoomType.IsCombatRoom())
                .Select(entry =>
                    new EncounterMetric(
                        (
                            entry.Rooms.Last().ModelId
                            ?? ModelId.none
                        ).Entry,
                        int.Min(
                            entry.GetEntry(localPlayerId).DamageTaken,
                            localPlayer.MaxHp),
                        entry.Rooms.Last().TurnsTaken + 1))
                .ToList();

        List<CardChoiceMetric> cardChoices =
            history
                .Where(entry =>
                    entry.GetEntry(localPlayerId)
                        .CardChoices.Count > 0)
                .Select(entry =>
                    new CardChoiceMetric(
                        entry.GetEntry(localPlayerId)
                            .CardChoices))
                .ToList();

        List<AncientMetric> ancientChoices =
            history
                .Where(entry =>
                    entry.MapPointType ==
                    MapPointType.Ancient)
                .Where(entry =>
                    entry.GetEntry(localPlayerId)
                        .AncientChoices.Count > 0)
                .Select(entry =>
                    new AncientMetric(
                        entry,
                        entry.GetEntry(localPlayerId)))
                .ToList();

        List<ActWinMetric> actWins = [];
        List<EventChoiceMetric> eventChoices = [];

        for (int index = 0;
             index < run.MapPointHistory.Count;
             index++)
        {
            eventChoices.AddRange(
                run.MapPointHistory[index]
                    .Where(entry =>
                        entry.Rooms.Count > 0 &&
                        entry.Rooms.First().RoomType ==
                        RoomType.Event &&
                        entry.GetEntry(localPlayerId)
                            .EventChoices.Count > 0 &&
                        entry.MapPointType !=
                        MapPointType.Ancient)
                    .Select(entry =>
                        new EventChoiceMetric(
                            entry,
                            localPlayerId,
                            run.Acts[index])));

            bool actWon =
                index <
                run.MapPointHistory.Count - 1
                || isVictory;

            string? actId =
                run.Acts[index].Id?.Entry;

            if (actId != null)
            {
                actWins.Add(
                    new ActWinMetric(
                        actId,
                        actWon));
            }
        }

        return new RunMetrics
        {
            Ascension = run.Ascension,

            NumReloads = run.NumReloads,

            BuildId =
                ReleaseInfoManager
                    .Instance
                    .ReleaseInfo?
                    .Version
                ?? "NON-RELEASE-VERSION",

            BuildType =
                PlatformUtil
                    .GetPlatformBranch()
                    .ToName(),

            // No persistent player identifier.
            PlayerId = string.Empty,

            Character =
                localPlayer.CharacterId
                ?? ModelId.none,

            NumPlayers =
                run.Players.Count,

            Team =
                run.Players.Count > 1
                    ? run.Players
                        .Select(player =>
                            player.CharacterId
                            ?? ModelId.none)
                        .ToList()
                    : [],

            Win =
                isVictory,

            FloorReached =
                history.Count,

            KilledByEncounter =
                killedBy,

            Deck =
                localPlayer.Deck.Select(
                    card =>
                        card.Id
                        ?? ModelId.none),

            Relics =
                localPlayer.Relics.Select(
                    relic =>
                        relic.Id
                        ?? ModelId.none),

            RunPlaytime =
                run.WinTime > 0
                    ? run.WinTime
                    : run.RunTime,

            Encounters =
                encounters,

            CardChoices =
                cardChoices,

            EventChoices =
                eventChoices,

            AncientChoices =
                ancientChoices,

            ActWins =
                actWins,

            CampfireUpgrades =
                history
                    .Where(entry =>
                        entry.MapPointType ==
                        MapPointType.RestSite)
                    .SelectMany(entry =>
                        entry.GetEntry(localPlayerId)
                            .UpgradedCards)
                    .Select(card =>
                        card.Entry)
                    .ToList(),

            RelicBuys =
                history
                    .SelectMany(entry =>
                        entry.GetEntry(localPlayerId)
                            .BoughtRelics)
                    .Select(relic =>
                        relic.Entry)
                    .ToList(),

            PotionBuys =
                history
                    .SelectMany(entry =>
                        entry.GetEntry(localPlayerId)
                            .BoughtPotions)
                    .Select(potion =>
                        potion.Entry)
                    .ToList(),

            ColorlessBuys =
                history
                    .SelectMany(entry =>
                        entry.GetEntry(localPlayerId)
                            .BoughtColorless)
                    .Select(card =>
                        card.Entry)
                    .ToList(),

            PotionDiscards =
                history
                    .SelectMany(entry =>
                        entry.GetEntry(localPlayerId)
                            .PotionDiscarded)
                    .Select(potion =>
                        potion.Entry)
                    .ToList(),

            // Account-wide stats aren't useful for Engineer balancing.
            TotalPlaytime = 0,
            TotalWinRate = 0
        };
    }

    private static bool HasForeignContent(
        SerializableRun run)
    {
        // Custom acts
        if (run.Acts.Any(
                act =>
                    !IsAllowed<ActModel>(act.Id)))
        {
            return true;
        }

        // Custom encounters / rooms
        if (run.MapPointHistory
            .SelectMany(entries => entries)
            .SelectMany(entry => entry.Rooms)
            .Any(room =>
                !IsAllowed<AbstractModel>(
                    room.ModelId)))
        {
            return true;
        }

        foreach (SerializablePlayer player in run.Players)
        {
            // Custom characters
            if (!IsAllowed<CharacterModel>(
                    player.CharacterId))
            {
                return true;
            }

            // Custom cards
            if (player.Deck.Any(card =>
                    !IsAllowed<CardModel>(
                        card.Id)))
            {
                return true;
            }

            // Custom relics
            if (player.Relics.Any(relic =>
                    !IsAllowed<RelicModel>(
                        relic.Id)))
            {
                return true;
            }

            // Custom potions
            if (player.Potions.Any(potion =>
                    !IsAllowed<PotionModel>(
                        potion.Id)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsAllowed<T>(
        ModelId? id)
        where T : AbstractModel
    {
        if (id is null ||
            id == ModelId.none)
        {
            return true;
        }

        T? model =
            ModelDb.GetByIdOrNull<T>(id);

        return model != null &&
               AllowedAssemblies.Contains(
                   model.GetType().Assembly);
    }

    private static string GetModVersion()
    {
        var mod =
            ModManager
                .GetLoadedMods()
                .FirstOrDefault(
                    loaded =>
                        loaded.manifest?.id ==
                        "TheEngineer");

        return mod?.manifest?.version
               ?? "unknown";
    }
}