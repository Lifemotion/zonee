using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
        return await _http.GetFromJsonAsync("domain", ZoneJsonContext.Default.ListDomain, ct) ?? [];
    }

    /// <summary>
    /// Gets detailed information about a domain.
    /// API returns an array — this returns the first element.
    /// </summary>
    public async Task<Domain?> GetAsync(string domain, CancellationToken ct = default)
    {
        var list = await _http.GetFromJsonAsync($"domain/{domain}", ZoneJsonContext.Default.ListDomain, ct);
        return list?.FirstOrDefault();
    }

    /// <summary>
    /// Renews or reactivates a domain.
    /// API expects an array body: [{"domain":"...", "period":1}]
    /// </summary>
    public async Task RenewAsync(DomainRenew request, CancellationToken ct = default)
    {
        var body = new List<DomainRenew> { request };
        var json = JsonSerializer.Serialize(body, ZoneJsonContext.Default.ListDomainRenew);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync("order/domain/renew", content, ct);
        await resp.EnsureZoneSuccess(ct);
    }
}
