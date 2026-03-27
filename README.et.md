Saadaval ka: [English](README.md) | [Eesti](README.et.md) | [Русский](README.ru.md)

# ZoneEe

Mitteametlik .NET klientteek ja käsurearakendus [Zone.eu API v2](https://api.zone.eu/v2) jaoks.

Halda oma domeene ja DNS-kirjeid terminalist või mis tahes .NET rakendusest.

> **Vastutusest loobumine:** Tegemist on kogukonnaprojektiga, mis ei ole seotud ettevõttega [Zone Media OÜ](https://www.zone.eu), ei ole selle poolt heaks kiidetud ega toetatud. Kasutamine omal vastutusel.

## Võimalused

- **DNS-i haldamine** -- kirjete loetlemine, loomine, muutmine ja kustutamine (A, AAAA, CNAME, MX, TXT, SRV, NS, CAA, SSHFP, URL, TLSA)
- **Domeenide haldamine** -- loetlemine, üksikasjade vaatamine, pikendamine
- **Krüpteeritud mandaadid** -- AES-256 krüpteeritud konfiguratsioon PIN-koodiga
- **Inimloetav väljund** -- värvikoodidega tabelid koos aegumishoiatustega
- **Masinloetav väljund** -- lipp `--json` skriptimiseks
- **Ohutud toimingud** -- lipp `--dry-run` andmeid muutvate käskude jaoks
- **Päringupiirangu käsitlus** -- automaatne uuesti proovimine HTTP 429 korral
- **Automaatne lehekülgede laadimine** -- kõik leheküljed laaditakse läbipaistvalt
- **Natiivne AOT** -- kiire käivitus, väike iseseisev binaarfail (~5 MB)
- **Kahene kasutus** -- käsurearakendus + NuGet teek .NET projektide jaoks

---

## Käsurearakendus (zonee)

### Paigaldamine

**Dotnet tööriistana:**

Nõuab .NET 10. Paigalda vajadusel:

- **Windows:** `winget install Microsoft.DotNet.SDK.10`
- **Linux** (snap-paketid ei tööta, kasuta ametlikku skripti):

```bash
curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 10.0
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.bashrc
source ~/.bashrc
```

Seejärel paigalda tööriist:

```
dotnet tool install -g Lfmt.Zonee.Cli
```

**Iseseisva binaarfailina** (.NET-i pole vaja):

Laadi alla [Releases](https://github.com/Lifemotion/zonee/releases) lehelt (Windows x64, Linux x64, Linux ARM64).

### Seadistamine

**Interaktiivne häälestus (soovitatav):**

```bash
zonee auth login
```

Küsib Zone.eu kasutajanime, API-võtit ja PIN-koodi. Mandaadid salvestatakse krüpteeritult (AES-256, PBKDF2 võtmetuletus) faili `~/.zonee/config`. Iga järgnev käsk küsib PIN-koodi.

> API-võtmed luuakse [Zone.eu konto seadetes](https://my.zone.eu) jaotises Turvalisus > API-võtmed.

**Keskkonnamuutujad (CI/CD, Docker, skriptid):**

```bash
export ZONE_USER=your-username
export ZONE_APIKEY=your-api-key
```

Keskkonnamuutujad on konfiguratsioonifailist kõrgema prioriteediga ja ei nõua PIN-koodi.

Krüpteeritud konfiguratsiooni kasutamiseks ilma interaktiivse PIN-koodi päringuta:

```bash
export ZONE_PIN=your-pin
```

**Salvestatud mandaatide eemaldamine:**

```bash
zonee auth logout
```

### DNS-käsud

```bash
# Kuva KÕIK domeeni kirjed
zonee dns list example.com

# Kuva kindlat tüüpi kirjed
zonee dns list example.com a
zonee dns list example.com mx
zonee dns list example.com txt

# Loo kirje
zonee dns add example.com a www.example.com 1.2.3.4

# Muuda kirjet ID järgi
zonee dns update example.com a 12345 2.3.4.5

# Kustuta kirje ID järgi
zonee dns delete example.com a 12345

# Eelvaade: mida saadetaks (tegelikku päringut ei tehta)
zonee dns add example.com a www.example.com 1.2.3.4 --dry-run

# Väljund JSON-vormingus
zonee dns list example.com --json
```

Toetatud kirjetüübid: `a`, `aaaa`, `cname`, `mx`, `txt`, `srv`, `ns`, `caa`, `sshfp`, `url`, `tlsa`.

### Domeeni käsud

```bash
# Kuva kõik domeenid (värvikoodidega aegumiskuupäevad)
zonee domain list

# Filtreeri nime järgi
zonee domain list --filter example

# Kuva ainult 30 päeva jooksul aeguvad domeenid
zonee domain list --expiring

# Kuva domeeni üksikasjad
zonee domain info example.com

# Pikenda domeeni 1 aastaks
zonee domain renew example.com

# Pikenda 2 aastaks
zonee domain renew example.com --period 2
```

### Üldised valikud

| Valik | Kirjeldus |
|---|---|
| `--json` | Väljund JSON-vormingus tabeli asemel |
| `--dry-run` | Näita, mida saadetaks, ilma päringut tegemata |
| `--version` | Kuva versioon |
| `--help` | Kuva abi |

### Päringupiirang

Zone.eu API lubab **60 päringut minutis** IP-aadressi kohta. Tööriist proovib automaatselt uuesti HTTP 429 korral, kuid kasutage vastutustundlikult:

- Ärge küsitlege API-t tsüklis -- DNS-i levik võtab aega ja seda ei saa selle API kaudu kontrollida
- `zonee dns list <domain>` (kõik tüübid) teeb korraga ~11 päringut -- kasutage võimalusel konkreetset tüüpi
- Kasutage `--dry-run` käskude kontrollimiseks enne täitmist
- Ärge küsige uuesti andmeid, mis teil juba on

---

## Teek (Lfmt.Zonee)

```
dotnet add package Lfmt.Zonee
```

### Kiirstart

```csharp
using ZoneEe;
using ZoneEe.Models;

using var client = new ZoneClient("username", "api-key");

// Kuva kõik domeenid
var domains = await client.Domains.ListAsync();
foreach (var d in domains)
    Console.WriteLine($"{d.Name} expires {d.Expires}");

// Domeeni üksikasjade päring
var domain = await client.Domains.GetAsync("example.com");

// Kuva kõik DNS A-kirjed
var records = await client.Dns.ListAsync("example.com", DnsRecordType.A);

// Loo DNS-kirje
var created = await client.Dns.CreateAsync("example.com", DnsRecordType.A, new DnsRecordCreate
{
    Name = "www.example.com",
    Destination = "1.2.3.4"
});

// Loo MX-kirje prioriteediga
await client.Dns.CreateAsync("example.com", DnsRecordType.MX, new DnsRecordCreate
{
    Name = "example.com",
    Destination = "mail.example.com",
    Priority = 10
});

// Muuda kirjet
await client.Dns.UpdateAsync("example.com", DnsRecordType.A, "12345", new DnsRecordUpdate
{
    Destination = "5.6.7.8"
});

// Kustuta kirje
await client.Dns.DeleteAsync("example.com", DnsRecordType.A, "12345");

// Pikenda domeeni
await client.Domains.RenewAsync(new DomainRenew { Domain = "example.com", Period = 1 });
```

### Seadistamisvalikud

```csharp
// Valikute objekti kasutamine
var client = new ZoneClient(new ZoneClientOptions
{
    Username = "your-username",
    ApiKey = "your-api-key",
    BaseUrl = "https://api.zone.eu/v2"  // vaikeväärtus
});

// Väliselt hallatud HttpClient (sõltuvussüsti / testimise jaoks)
var httpClient = new HttpClient { BaseAddress = new Uri("https://api.zone.eu/v2/") };
var client = new ZoneClient(httpClient);
```

### Sisseehitatud võimalused

- **Päringupiirangu käsitlus** -- automaatne uuesti proovimine HTTP 429 korral (kuni 3 korda kasvava ooteajaga)
- **Automaatne lehekülgede laadimine** -- `ListAsync` meetodid laadivad kõik leheküljed läbipaistvalt
- **Üksikasjalikud veateated** -- `ZoneApiException` sisaldab olekukoodi, vastuse sisu ja Zone.eu olekuteadet

---

## Levinud stsenaariumid

### Domeeni ühendamine Azure App Service'iga

```bash
# Domeeni omandiõiguse kinnitamine
zonee dns add example.com txt asuid.www.example.com "<verification-id>"

# Suuna www Azure'i
zonee dns add example.com cname www.example.com myapp.azurewebsites.net

# Suuna juurdomeeni Azure'i IP-aadressile
zonee dns add example.com a example.com <azure-ip>
zonee dns add example.com txt asuid.example.com "<verification-id>"
```

### Domeeni ühendamine Azure Static Web Apps'iga

```bash
zonee dns add example.com cname www.example.com <app>.azurestaticapps.net
zonee dns add example.com txt example.com "<validation-token>"
zonee dns add example.com a example.com <azure-static-ip>
```

### Domeeni ühendamine Verceliga

```bash
zonee dns add example.com a example.com 76.76.21.21
zonee dns add example.com cname www.example.com cname.vercel-dns.com
```

### Let's Encrypt DNS-01 väljakutse

```bash
zonee dns add example.com txt _acme-challenge.example.com "<token>"
# Pärast sertifikaadi väljastamist:
zonee dns delete example.com txt <record-id>
```

### Kasutamine Claude Code'iga

Kopeerige [`docs/CLAUDE-DNS-TEMPLATE.md`](docs/CLAUDE-DNS-TEMPLATE.md) oma projekti `CLAUDE.md` failina (või lisage olemasolevale), et Claude Code teaks, kuidas teie domeeni DNS-kirjeid hallata.

---

## Lähtekoodist ehitamine

```bash
git clone https://github.com/Lifemotion/zonee.git
cd zonee
dotnet build
```

**Natiivse AOT binaarfaili avaldamine:**

```bash
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r win-x64
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r linux-x64
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r linux-arm64
```

## Litsents

MIT -- Sasha Sovenko
