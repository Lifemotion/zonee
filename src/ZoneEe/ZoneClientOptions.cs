namespace ZoneEe;

/// <summary>
/// Configuration options for <see cref="ZoneClient"/>.
/// </summary>
public class ZoneClientOptions
{
    /// <summary>ZoneID account name.</summary>
    public required string Username { get; set; }

    /// <summary>API key generated in ZoneID account settings.</summary>
    public required string ApiKey { get; set; }

    /// <summary>Base URL of the Zone.eu API. Defaults to production.</summary>
    public string BaseUrl { get; set; } = "https://api.zone.eu/v2";
}
