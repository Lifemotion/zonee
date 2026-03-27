namespace ZoneEe.Services;

internal static class HttpResponseExtensions
{
    public static async Task EnsureZoneSuccess(this HttpResponseMessage resp, CancellationToken ct = default)
    {
        if (resp.IsSuccessStatusCode)
            return;

        var body = await resp.Content.ReadAsStringAsync(ct);
        resp.Headers.TryGetValues("X-Status-Message", out var statusMessages);
        var statusMessage = statusMessages?.FirstOrDefault();

        throw new ZoneApiException(resp.StatusCode, body, statusMessage);
    }
}
