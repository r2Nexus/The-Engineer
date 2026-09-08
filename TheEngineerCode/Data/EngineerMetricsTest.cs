using System.Text.Json;

namespace TheEngineer.TheEngineerCode.Data;

internal static class EngineerMetricsTest
{
    public static async Task SendTest()
    {
        var payload = new
        {
            schema_version = 1,
            mod_version = "TEST",
            has_foreign_content = false,
            data = new
            {
                test = true,
                message = "Hello from The Engineer"
            }
        };

        string json = JsonSerializer.Serialize(payload);

        await EngineerMetricsClient.Upload(json);
    }
}