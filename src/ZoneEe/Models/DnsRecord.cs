using System.Text.Json.Serialization;

namespace ZoneEe.Models;

public class DnsRecord
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

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
}

public class DnsRecordCreate
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("destination")]
    public string Destination { get; set; } = "";
}

public class DnsRecordUpdate
{
    [JsonPropertyName("destination")]
    public string Destination { get; set; } = "";
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
    [JsonStringEnumMemberName("soa")]
    SOA
}
