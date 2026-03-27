# ZoneEe — development notes

## Project structure

- `src/ZoneEe/` — .NET library (NuGet: Lfmt.Zonee). Zero dependencies beyond BCL.
- `src/ZoneEe.Cli/` — CLI tool (binary: `zonee`). Uses Spectre.Console.Cli.
- Solution: `ZoneEe.sln`, shared settings: `Directory.Build.props` (.NET 10.0).

## Build & run

```bash
dotnet build                                          # build all
dotnet run --project src/ZoneEe.Cli -- <args>         # run CLI
dotnet publish src/ZoneEe.Cli -c Release -r win-x64   # native AOT binary
```

## Architecture

- Library has no CLI dependencies. All Spectre/output/auth logic stays in CLI project.
- API responses are always JSON arrays — services unwrap with `.FirstOrDefault()`.
- JSON serialization uses source-generated contexts (`ZoneJsonContext`, `AppJsonContext`) for AOT.
- POST/PUT use `StringContent` (not `PostAsJsonAsync`) — Zone.eu API rejects the latter.
- Rate limiting: `RateLimitHandler` retries HTTP 429 automatically.
- Pagination: `PaginationHelper.GetAllPagesAsync` fetches all pages via `x-pager-*` headers.

## Zone.eu API

- Base: `https://api.zone.eu/v2`
- Auth: HTTP Basic (username + API key)
- Rate limit: 60 req/min per IP. Don't poll, don't make redundant requests, cache results locally
- Full OpenAPI spec: see `swagger.json` (obtained from api.zone.eu/v2 ReDoc page)
- Domain registration is NOT available via API — only through web UI
- Domain renewal: `POST /v2/order/domain/renew` with array body `[{"domain":"...","period":1}]`

## Testing

No sandbox environment exists. Test with:
- Read-only operations (`domain list`, `dns list`) — safe
- `--dry-run` for mutating operations
- Real mutations only on designated test domains (e.g., gosplan.eu)

## Auth in dev

```bash
export ZONE_PIN=1321    # skip interactive PIN prompt
# or
export ZONE_USER=...
export ZONE_APIKEY=...
```
