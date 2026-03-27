# ZoneEe

.NET client library and CLI for the [Zone.eu API v2](https://api.zone.eu/v2).

Manage your domains and DNS records from the terminal or from any .NET application.

## Features

- **DNS management** -- list, create, update, delete records (A, AAAA, CNAME, MX, TXT, SRV, NS, CAA, SSHFP, URL, TLSA)
- **Domain management** -- list, view details, renew
- **Encrypted credentials** -- AES-256 encrypted config with PIN protection
- **Human-friendly output** -- color-coded tables with expiry warnings
- **Machine-friendly output** -- `--json` flag for scripting
- **Safe operations** -- `--dry-run` flag for mutating commands
- **Rate limit handling** -- automatic retry on HTTP 429
- **Auto-pagination** -- fetches all pages transparently
- **Native AOT** -- fast startup, small self-contained binary (~5 MB)
- **Dual-use** -- CLI tool + NuGet library for .NET projects

---

## CLI (zonee)

### Installation

**As a dotnet tool:**

```
dotnet tool install -g Lfmt.Zonee.Cli
```

**As a standalone binary:**

Download from [Releases](https://github.com/Lifemotion/zonee/releases) (Windows x64, Linux x64, Linux ARM64).

### Configuration

**Interactive setup (recommended):**

```bash
zonee auth login
```

Prompts for Zone.eu username, API key, and a PIN code. Credentials are stored encrypted (AES-256, PBKDF2 key derivation) in `~/.zonee/config`. Every subsequent command will ask for the PIN.

> API keys are generated in [Zone.eu account settings](https://my.zone.eu) under Security > API keys.

**Environment variables (for CI/CD, Docker, scripts):**

```bash
export ZONE_USER=your-username
export ZONE_APIKEY=your-api-key
```

Environment variables take priority over the config file and bypass PIN.

To use the encrypted config without interactive PIN prompt:

```bash
export ZONE_PIN=your-pin
```

**Remove saved credentials:**

```bash
zonee auth logout
```

### DNS commands

```bash
# List ALL records for a domain
zonee dns list example.com

# List records of a specific type
zonee dns list example.com a
zonee dns list example.com mx
zonee dns list example.com txt

# Create a record
zonee dns add example.com a www.example.com 1.2.3.4

# Update a record by ID
zonee dns update example.com a 12345 2.3.4.5

# Delete a record by ID
zonee dns delete example.com a 12345

# Preview what would be sent (no actual request)
zonee dns add example.com a www.example.com 1.2.3.4 --dry-run

# Output as JSON
zonee dns list example.com --json
```

Supported record types: `a`, `aaaa`, `cname`, `mx`, `txt`, `srv`, `ns`, `caa`, `sshfp`, `url`, `tlsa`.

### Domain commands

```bash
# List all domains (with color-coded expiry dates)
zonee domain list

# Filter by name
zonee domain list --filter example

# Show only domains expiring within 30 days
zonee domain list --expiring

# Show domain details
zonee domain info example.com

# Renew a domain for 1 year
zonee domain renew example.com

# Renew for 2 years
zonee domain renew example.com --period 2
```

### Global options

| Option | Description |
|---|---|
| `--json` | Output as JSON instead of a table |
| `--dry-run` | Show what would be sent without making the request |
| `--version` | Print version |
| `--help` | Print help |

---

## Library (Lfmt.Zonee)

```
dotnet add package Lfmt.Zonee
```

### Quick start

```csharp
using ZoneEe;
using ZoneEe.Models;

using var client = new ZoneClient("username", "api-key");

// List all domains
var domains = await client.Domains.ListAsync();
foreach (var d in domains)
    Console.WriteLine($"{d.Name} expires {d.Expires}");

// Get domain details
var domain = await client.Domains.GetAsync("example.com");

// List all DNS A records
var records = await client.Dns.ListAsync("example.com", DnsRecordType.A);

// Create a DNS record
var created = await client.Dns.CreateAsync("example.com", DnsRecordType.A, new DnsRecordCreate
{
    Name = "www.example.com",
    Destination = "1.2.3.4"
});

// Create an MX record with priority
await client.Dns.CreateAsync("example.com", DnsRecordType.MX, new DnsRecordCreate
{
    Name = "example.com",
    Destination = "mail.example.com",
    Priority = 10
});

// Update a record
await client.Dns.UpdateAsync("example.com", DnsRecordType.A, "12345", new DnsRecordUpdate
{
    Destination = "5.6.7.8"
});

// Delete a record
await client.Dns.DeleteAsync("example.com", DnsRecordType.A, "12345");

// Renew a domain
await client.Domains.RenewAsync(new DomainRenew { Domain = "example.com", Period = 1 });
```

### Configuration options

```csharp
// Using options object
var client = new ZoneClient(new ZoneClientOptions
{
    Username = "your-username",
    ApiKey = "your-api-key",
    BaseUrl = "https://api.zone.eu/v2"  // default
});

// Using an externally-managed HttpClient (for DI / testing)
var httpClient = new HttpClient { BaseAddress = new Uri("https://api.zone.eu/v2/") };
var client = new ZoneClient(httpClient);
```

### Built-in features

- **Rate limit handling** -- automatically retries on HTTP 429 (up to 3 times with backoff)
- **Auto-pagination** -- `ListAsync` methods fetch all pages transparently
- **Detailed errors** -- `ZoneApiException` includes status code, response body, and Zone.eu's status message

---

## Building from source

```bash
git clone https://github.com/Lifemotion/zonee.git
cd zonee
dotnet build
```

**Publish native AOT binary:**

```bash
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r win-x64
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r linux-x64
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r linux-arm64
```

## License

MIT -- Sasha Sovenko
