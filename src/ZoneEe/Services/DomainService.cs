using System.Net.Http.Json;
using ZoneEe.Models;

namespace ZoneEe.Services;

/// <summary>
/// Manages domains via the Zone.eu API.
/// </summary>
public class DomainService
{
    private readonly HttpClient _http;

    internal DomainService(HttpClient http) => _http = http;

    /// <summary>
    /// Lists all domains under the account.
    /// </summary>
    public async Task<List<Domain>> ListAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<List<Domain>>("domain", ct) ?? [];
    }

    /// <summary>
    /// Gets detailed information about a domain.
    /// </summary>
    public async Task<DomainDetail?> GetAsync(string domain, CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<DomainDetail>($"domain/{domain}", ct);
    }

    /// <summary>
    /// Registers a new domain.
    /// </summary>
    public async Task<DomainDetail?> RegisterAsync(DomainRegister request, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("domain", request, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<DomainDetail>(ct);
    }
}
