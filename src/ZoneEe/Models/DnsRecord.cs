using System.Text.Json.Serialization;

namespace ZoneEe.Models;

/// <summary>
/// DNS record returned by the Zone.eu API.
/// Contains base fields common to all record types plus optional
/// type-specific fields (priority, weight, port, flag, tag, etc.).
/// </summary>
public class DnsRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("resource_url")]
    public string ResourceUrl { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("destination")]
    public string Destination { get; set; } = "";

    [JsonPropertyName("delete")]
    public bool CanDelete { get; set; }

    [JsonPropertyName("modify")]
    public bool CanModify { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    // MX, SRV
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    // SRV
    [JsonPropertyName("weight")]
    public int? Weight { get; set; }

    [JsonPropertyName("port")]
    public int? Port { get; set; }

    // CAA
    [JsonPropertyName("flag")]
    public int? Flag { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    // SSHFP
    [JsonPropertyName("algorithm")]
    public int? Algorithm { get; set; }

    [JsonPropertyName("type")]
    public int? FingerprintType { get; set; }

    // TLSA
    [JsonPropertyName("certificate_usage")]
    public int? CertificateUsage { get; set; }

    [JsonPropertyName("selector")]
    public int? Selector { get; set; }

    [JsonPropertyName("matching_type")]
    public int? MatchingType { get; set; }

    // URL redirect
    [JsonPropertyName("redirect_type")]
    public int? RedirectType { get; set; }
}

/// <summary>
/// Request body for creating a DNS record.
/// Include only the fields relevant to the record type.
/// </summary>
public class DnsRecordCreate
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("destination")]
    public string Destination { get; set; } = "";

    // MX, SRV
    [JsonPropertyName("priority")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Priority { get; set; }

    // SRV
    [JsonPropertyName("weight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Weight { get; set; }

    [JsonPropertyName("port")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Port { get; set; }

    // CAA
    [JsonPropertyName("flag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Flag { get; set; }

    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; set; }

    // SSHFP
    [JsonPropertyName("algorithm")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Algorithm { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? FingerprintType { get; set; }

    // TLSA
    [JsonPropertyName("certificate_usage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CertificateUsage { get; set; }

    [JsonPropertyName("selector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Selector { get; set; }

    [JsonPropertyName("matching_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MatchingType { get; set; }

    // URL redirect
    [JsonPropertyName("redirect_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RedirectType { get; set; }
}

/// <summary>
/// Request body for updating a DNS record.
/// </summary>
public class DnsRecordUpdate
{
    [JsonPropertyName("destination")]
    public string Destination { get; set; } = "";

    [JsonPropertyName("priority")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Priority { get; set; }

    [JsonPropertyName("weight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Weight { get; set; }

    [JsonPropertyName("port")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Port { get; set; }

    [JsonPropertyName("flag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Flag { get; set; }

    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; set; }

    [JsonPropertyName("algorithm")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Algorithm { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? FingerprintType { get; set; }

    [JsonPropertyName("certificate_usage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CertificateUsage { get; set; }

    [JsonPropertyName("selector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Selector { get; set; }

    [JsonPropertyName("matching_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MatchingType { get; set; }

    [JsonPropertyName("redirect_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RedirectType { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<DnsRecordType>))]
public enum DnsRecordType
{
    [JsonStringEnumMemberName("a")]
    A,
    [JsonStringEnumMemberName("aaaa")]
    AAAA,
    [JsonStringEnumMemberName("cname")]
    CNAME,
    [JsonStringEnumMemberName("mx")]
    MX,
    [JsonStringEnumMemberName("txt")]
    TXT,
    [JsonStringEnumMemberName("srv")]
    SRV,
    [JsonStringEnumMemberName("ns")]
    NS,
    [JsonStringEnumMemberName("caa")]
    CAA,
    [JsonStringEnumMemberName("sshfp")]
    SSHFP,
    [JsonStringEnumMemberName("url")]
    URL,
    [JsonStringEnumMemberName("tlsa")]
    TLSA
}
