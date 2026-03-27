using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
        return await _http.GetFromJsonAsync(path, ZoneJsonContext.Default.ListDnsRecord, ct) ?? [];
    }

    /// <summary>
    /// Gets a single DNS record by ID.
    /// API returns an array — this returns the first element.
    /// </summary>
    public async Task<DnsRecord?> GetAsync(string domain, DnsRecordType type, string recordId, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}/{recordId}";
        var list = await _http.GetFromJsonAsync(path, ZoneJsonContext.Default.ListDnsRecord, ct);
        return list?.FirstOrDefault();
    }

    /// <summary>
    /// Creates a new DNS record.
    /// </summary>
    public async Task<DnsRecord?> CreateAsync(string domain, DnsRecordType type, DnsRecordCreate record, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}";
        var json = JsonSerializer.Serialize(record, ZoneJsonContext.Default.DnsRecordCreate);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(path, content, ct);
        await resp.EnsureZoneSuccess(ct);
        var list = await resp.Content.ReadFromJsonAsync(ZoneJsonContext.Default.ListDnsRecord, ct);
        return list?.FirstOrDefault();
    }

    /// <summary>
    /// Updates an existing DNS record.
    /// </summary>
    public async Task<DnsRecord?> UpdateAsync(string domain, DnsRecordType type, string recordId, DnsRecordUpdate record, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}/{recordId}";
        var json = JsonSerializer.Serialize(record, ZoneJsonContext.Default.DnsRecordUpdate);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PutAsync(path, content, ct);
        await resp.EnsureZoneSuccess(ct);
        var list = await resp.Content.ReadFromJsonAsync(ZoneJsonContext.Default.ListDnsRecord, ct);
        return list?.FirstOrDefault();
    }

    /// <summary>
    /// Deletes a DNS record by ID.
    /// </summary>
    public async Task DeleteAsync(string domain, DnsRecordType type, string recordId, CancellationToken ct = default)
    {
        var path = $"dns/{domain}/{ToPath(type)}/{recordId}";
        var resp = await _http.DeleteAsync(path, ct);
        await resp.EnsureZoneSuccess(ct);
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
