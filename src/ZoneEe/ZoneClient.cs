using System.Net.Http.Headers;
using System.Text;
using ZoneEe.Services;

namespace ZoneEe;

/// <summary>
/// Client for the Zone.eu API v2.
/// </summary>
public class ZoneClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly bool _ownsClient;

    /// <summary>DNS record management.</summary>
    public DnsService Dns { get; }

    /// <summary>Domain management.</summary>
    public DomainService Domains { get; }

    /// <summary>
    /// Creates a new client with the given credentials.
    /// </summary>
    public ZoneClient(string username, string apiKey, string? baseUrl = null)
        : this(new ZoneClientOptions
        {
            Username = username,
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.zone.eu/v2"
        })
    {
    }

    /// <summary>
    /// Creates a new client with the given options.
    /// </summary>
    public ZoneClient(ZoneClientOptions options)
        : this(CreateHttpClient(options), ownsClient: true)
    {
    }

    /// <summary>
    /// Creates a new client with an externally-managed <see cref="HttpClient"/>.
    /// Useful for dependency injection and testing.
    /// </summary>
    public ZoneClient(HttpClient httpClient)
        : this(httpClient, ownsClient: false)
    {
    }

    private ZoneClient(HttpClient httpClient, bool ownsClient)
    {
        _http = httpClient;
        _ownsClient = ownsClient;
        Dns = new DnsService(_http);
        Domains = new DomainService(_http);
    }

    private static HttpClient CreateHttpClient(ZoneClientOptions options)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/")
        };

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{options.Username}:{options.ApiKey}"));

        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        return http;
    }

    public void Dispose()
    {
        if (_ownsClient) _http.Dispose();
        GC.SuppressFinalize(this);
    }
}
