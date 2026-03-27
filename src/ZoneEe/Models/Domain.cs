using System.Text.Json.Serialization;

namespace ZoneEe.Models;

public class Domain
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("expires_at")]
    public string? ExpiresAt { get; set; }

    [JsonPropertyName("autorenew")]
    public bool AutoRenew { get; set; }
}

public class DomainDetail
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("expires_at")]
    public string? ExpiresAt { get; set; }

    [JsonPropertyName("autorenew")]
    public bool AutoRenew { get; set; }

    [JsonPropertyName("registrant")]
    public string? Registrant { get; set; }

    [JsonPropertyName("admin_contact")]
    public string? AdminContact { get; set; }

    [JsonPropertyName("nameservers")]
    public List<string>? Nameservers { get; set; }
}

public class DomainRegister
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("period")]
    public int Period { get; set; } = 1;
}
