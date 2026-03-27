using System.Net.Http.Json;
using ZoneEe.Models;

namespace ZoneEe.Services;

/// <summary>
/// Manages DNS records via the Zone.eu API.
/// </summary>
public class DnsService
{
    private readonly HttpClient _http;

    internal DnsService(HttpClient http) => _http = http;

    /// <summary>
    /// Lists all DNS records of the given type for a domain.
    /// </summary>
    public async Task<List<DnsRecord>> ListAsync(string domain, DnsRecordType type, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}";
        return await _http.GetFromJsonAsync<List<DnsRecord>>(path, ct) ?? [];
    }

    /// <summary>
    /// Gets a single DNS record by ID.
    /// </summary>
    public async Task<DnsRecord?> GetAsync(string domain, DnsRecordType type, long recordId, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}/{recordId}";
        return await _http.GetFromJsonAsync<DnsRecord>(path, ct);
    }

    /// <summary>
    /// Creates a new DNS record.
    /// </summary>
    public async Task<DnsRecord?> CreateAsync(string domain, DnsRecordType type, DnsRecordCreate record, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}";
        var resp = await _http.PostAsJsonAsync(path, record, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<DnsRecord>(ct);
    }

    /// <summary>
    /// Updates an existing DNS record.
    /// </summary>
    public async Task<DnsRecord?> UpdateAsync(string domain, DnsRecordType type, long recordId, DnsRecordUpdate record, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}/{recordId}";
        var resp = await _http.PutAsJsonAsync(path, record, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<DnsRecord>(ct);
    }

    /// <summary>
    /// Deletes a DNS record by ID.
    /// </summary>
    public async Task DeleteAsync(string domain, DnsRecordType type, long recordId, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}/{recordId}";
        var resp = await _http.DeleteAsync(path, ct);
        resp.EnsureSuccessStatusCode();
    }

    private static string ToPath(DnsRecordType type) => type switch
    {
        DnsRecordType.A => "a",
        DnsRecordType.AAAA => "aaaa",
        DnsRecordType.CNAME => "cname",
        DnsRecordType.MX => "mx",
        DnsRecordType.TXT => "txt",
        DnsRecordType.SRV => "srv",
        DnsRecordType.NS => "ns",
        DnsRecordType.SOA => "soa",
        _ => type.ToString().ToLowerInvariant()
    };
}
