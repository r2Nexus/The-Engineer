namespace TheEngineer.TheEngineerCode.Data;

internal static class EngineerMetricsEndpoint
{
    // Supabase Project URL.
    public const string ProjectUrl =
        "https://xxx.supabase.co";
    
    // sb_publishable_...
    public const string PublishableKey =
        "sb_publishable_xxx";

    public static string RunsUrl =>
        $"{ProjectUrl}/rest/v1/engineer_runs";
}