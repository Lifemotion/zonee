using System.Net;

namespace ZoneEe.Services;

/// <summary>
/// DelegatingHandler that retries requests on HTTP 429 (Too Many Requests).
/// Respects Retry-After header, defaults to 2 seconds.
/// </summary>
internal class RateLimitHandler : DelegatingHandler
{
    private const int MaxRetries = 3;

    public RateLimitHandler() : base(new HttpClientHandler()) { }
    public RateLimitHandler(HttpMessageHandler inner) : base(inner) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        for (var attempt = 0; ; attempt++)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.TooManyRequests || attempt >= MaxRetries)
                return response;

            var delay = TimeSpan.FromSeconds(2);
            if (response.Headers.RetryAfter?.Delta is { } delta)
                delay = delta;
            else if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var values)
                     && int.TryParse(values.FirstOrDefault(), out var remaining) && remaining == 0)
                delay = TimeSpan.FromSeconds(5);

            await Task.Delay(delay, cancellationToken);
        }
    }
}
