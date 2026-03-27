# ZoneEe

.NET client library and CLI for the [Zone.eu API v2](https://api.zone.eu/v2).

## Library (Lfmt.Zonee)

```
dotnet add package Lfmt.Zonee
```

```csharp
using ZoneEe;

using var client = new ZoneClient("username", "api-key");

// List domains
var domains = await client.Domains.ListAsync();

// List DNS A records
var records = await client.Dns.ListAsync("example.com", DnsRecordType.A);

// Create a DNS record
await client.Dns.CreateAsync("example.com", DnsRecordType.A, new()
{
    Name = "www.example.com",
    Destination = "1.2.3.4"
});
```

## CLI (zonee)

```
dotnet tool install -g Lfmt.Zonee.Cli
```

Or download a standalone binary from [Releases](https://github.com/Lifemotion/zonee/releases).

### Configuration

**Interactive setup (recommended):**

```bash
zonee auth login
# Prompts for username, API key, and PIN
# Credentials are stored encrypted (AES-256) in ~/.zonee/config
```

Every subsequent command will ask for the PIN to decrypt credentials.

**Environment variables (for CI/CD, Docker, scripts):**

```bash
export ZONE_USER=your-username
export ZONE_APIKEY=your-api-key
```

Environment variables bypass PIN and take priority over the config file.

To avoid PIN prompts in scripts while using the config file:

```bash
export ZONE_PIN=your-pin
```

**Remove saved credentials:**

```bash
zonee auth logout
```

### Usage

```bash
# Domains
zonee domain list
zonee domain info example.com

# DNS records
zonee dns list example.com a
zonee dns add example.com a www.example.com 1.2.3.4
zonee dns update example.com a 12345 2.3.4.5
zonee dns delete example.com a 12345

# Dry run (shows request without sending)
zonee dns add example.com a www.example.com 1.2.3.4 --dry-run

# JSON output
zonee dns list example.com a --json
```

## License

MIT
