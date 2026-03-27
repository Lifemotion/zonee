using System.Text.Json.Serialization;

namespace ZoneEe.Models;

public class Domain
{
    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }

    [JsonPropertyName("identificator")]
    public string? Identificator { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("delegated")]
    public string? Delegated { get; set; }

    [JsonPropertyName("expires")]
    public string? Expires { get; set; }

    [JsonPropertyName("expired")]
    public bool Expired { get; set; }

    [JsonPropertyName("dnssec")]
    public bool Dnssec { get; set; }

    [JsonPropertyName("dnssec_supported")]
    public bool DnssecSupported { get; set; }

    [JsonPropertyName("autorenew")]
    public bool AutoRenew { get; set; }

    [JsonPropertyName("renew_order")]
    public string? RenewOrder { get; set; }

    [JsonPropertyName("renewal_notifications")]
    public bool RenewalNotifications { get; set; }

    [JsonPropertyName("has_pending_trade")]
    public int? HasPendingTrade { get; set; }

    [JsonPropertyName("has_pending_dnssec")]
    public bool HasPendingDnssec { get; set; }

    [JsonPropertyName("reactivate")]
    public bool Reactivate { get; set; }

    [JsonPropertyName("auth_key_enabled")]
    public bool AuthKeyEnabled { get; set; }

    [JsonPropertyName("signing_required")]
    public bool SigningRequired { get; set; }

    [JsonPropertyName("nameservers_custom")]
    public bool NameserversCustom { get; set; }

    [JsonPropertyName("_links")]
    public DomainLinks? Links { get; set; }
}

public class DomainRenew
{
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = "";

    [JsonPropertyName("period")]
    public int Period { get; set; } = 1;
}

public class DomainLinks
{
    [JsonPropertyName("nameserver")]
    public string? Nameserver { get; set; }

    [JsonPropertyName("contact")]
    public string? Contact { get; set; }

    [JsonPropertyName("webhosting")]
    public string? Webhosting { get; set; }
}
