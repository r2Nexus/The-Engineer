using System.Net.Http;
using System.Text;

namespace TheEngineer.TheEngineerCode.Data;

internal static class EngineerMetricsClient
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    public static async Task<bool> Upload(string json)
    {
        try
        {
            using HttpRequestMessage request = new(
                HttpMethod.Post,
                EngineerMetricsEndpoint.RunsUrl);

            request.Headers.Add(
                "apikey",
                EngineerMetricsEndpoint.PublishableKey);

            request.Headers.Add(
                "Prefer",
                "return=minimal");

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response =
                await HttpClient.SendAsync(request);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"[TheEngineer] Metrics upload succeeded: " +
                    $"{(int)response.StatusCode} {response.StatusCode}");

                return true;
            }

            Console.WriteLine(
                $"[TheEngineer] Metrics upload FAILED: " +
                $"{(int)response.StatusCode} {response.StatusCode}\n" +
                responseBody);

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[TheEngineer] Metrics upload exception:\n{ex}");

            return false;
        }
    }
}