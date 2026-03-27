# DNS management with zonee

This project uses [zonee](https://github.com/Lifemotion/zonee) CLI to manage DNS records on Zone.eu.

## Prerequisites

`zonee` must be installed as a global dotnet tool:

```bash
dotnet tool install -g Lfmt.Zonee.Cli
```

Credentials are configured via environment variables:

```bash
export ZONE_USER=<zoneid-username>
export ZONE_APIKEY=<zoneid-api-key>
```

Or via encrypted config (requires interactive `zonee auth login` first, then set `ZONE_PIN` env var for non-interactive use).

## How to use zonee

### List all DNS records for a domain

```bash
zonee dns list example.com
zonee dns list example.com a          # only A records
zonee dns list example.com --json     # machine-readable
```

### Create DNS records

```bash
zonee dns add <domain> <type> <name> <destination>

# Examples:
zonee dns add example.com a example.com 20.30.40.50
zonee dns add example.com cname www.example.com myapp.azurewebsites.net
zonee dns add example.com txt example.com "v=spf1 include:_spf.google.com ~all"
zonee dns add example.com txt _verify.example.com "azure-verification=ABC123"
```

### Update a DNS record

```bash
zonee dns list example.com a                        # find the record ID
zonee dns update example.com a <id> <new-value>     # update by ID
```

### Delete a DNS record

```bash
zonee dns delete example.com a <id>
```

### Safety

- Use `--dry-run` to preview changes: `zonee dns add ... --dry-run`
- Use `--json` to parse output programmatically
- Always run `zonee dns list <domain>` before and after changes to verify

## Common scenarios

### Connect domain to Azure App Service

```bash
# 1. Verify domain ownership
zonee dns add example.com txt asuid.www.example.com "<verification-id-from-azure>"

# 2. Point www to Azure
zonee dns add example.com cname www.example.com myapp.azurewebsites.net

# 3. Point root domain to Azure IP (from Azure Portal > App Service > Custom domains)
zonee dns add example.com a example.com <azure-app-service-ip>

# 4. Root domain verification
zonee dns add example.com txt asuid.example.com "<verification-id-from-azure>"
```

### Connect domain to Azure Static Web Apps

```bash
# 1. Point www to Azure
zonee dns add example.com cname www.example.com <app-name>.azurestaticapps.net

# 2. Root domain — add TXT for validation, then A record
zonee dns add example.com txt example.com "<validation-token-from-azure>"
zonee dns add example.com a example.com <azure-static-web-app-ip>
```

### Connect domain to Vercel

```bash
# 1. Root domain
zonee dns add example.com a example.com 76.76.21.21

# 2. www subdomain
zonee dns add example.com cname www.example.com cname.vercel-dns.com
```

### Connect domain to Cloudflare (as proxy/CDN)

```bash
# Change nameservers via Zone.eu web UI, then manage DNS in Cloudflare.
# Or keep Zone.eu nameservers and point to Cloudflare origin:
zonee dns add example.com a example.com <origin-server-ip>
zonee dns add example.com cname www.example.com <origin-server-hostname>
```

### Add Let's Encrypt DNS-01 challenge

```bash
zonee dns add example.com txt _acme-challenge.example.com "<challenge-token>"
# After certificate is issued:
zonee dns delete example.com txt <record-id>
```

### Set up email (MX records)

```bash
# Google Workspace
zonee dns add example.com mx example.com aspmx.l.google.com     # priority not yet supported in CLI args — use library
# Or manual approach via API

# General MX
zonee dns add example.com mx example.com mail.example.com
```

## Domain management

```bash
zonee domain list                      # all domains with expiry dates
zonee domain list --expiring           # domains expiring within 30 days
zonee domain info example.com          # detailed info
zonee domain renew example.com         # renew for 1 year
```

## Troubleshooting

- **"Credentials not found"** — run `zonee auth login` or set `ZONE_USER` + `ZONE_APIKEY`
- **"Wrong PIN"** — incorrect PIN for encrypted config
- **"Zone API error 422"** — invalid input, check record name/destination format
- **"Zone API error 429"** — rate limited, zonee retries automatically (max 3 times)
- **Rate limit is 60 requests/minute** — avoid rapid bulk operations
