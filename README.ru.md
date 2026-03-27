Также доступно на: [English](README.md) | [Eesti](README.et.md) | [Русский](README.ru.md)

# ZoneEe

Неофициальная клиентская библиотека .NET и CLI-утилита для [Zone.eu API v2](https://api.zone.eu/v2).

Управляйте доменами и DNS-записями из терминала или из любого .NET приложения.

> **Отказ от ответственности:** Это общественный проект, который не связан с компанией [Zone Media OÜ](https://www.zone.eu), не одобрен и не поддерживается ею. Используйте на свой страх и риск.

## Возможности

- **Управление DNS** -- просмотр, создание, обновление и удаление записей (A, AAAA, CNAME, MX, TXT, SRV, NS, CAA, SSHFP, URL, TLSA)
- **Управление доменами** -- просмотр списка, подробные сведения, продление
- **Зашифрованные учётные данные** -- конфигурация с шифрованием AES-256 и защитой PIN-кодом
- **Удобный вывод** -- цветные таблицы с предупреждениями об истечении срока
- **Машинночитаемый вывод** -- флаг `--json` для скриптов
- **Безопасные операции** -- флаг `--dry-run` для команд, изменяющих данные
- **Обработка лимита запросов** -- автоматический повтор при HTTP 429
- **Автопагинация** -- прозрачная загрузка всех страниц
- **Нативная AOT-компиляция** -- быстрый запуск, компактный автономный бинарник (~5 МБ)
- **Двойное назначение** -- CLI-утилита + NuGet-библиотека для .NET проектов

---

## CLI-утилита (zonee)

### Установка

**Как dotnet-инструмент:**

```
dotnet tool install -g Lfmt.Zonee.Cli
```

**Как автономный бинарник:**

Скачайте со страницы [Releases](https://github.com/Lifemotion/zonee/releases) (Windows x64, Linux x64, Linux ARM64).

### Настройка

**Интерактивная настройка (рекомендуется):**

```bash
zonee auth login
```

Запросит имя пользователя Zone.eu, API-ключ и PIN-код. Учётные данные сохраняются в зашифрованном виде (AES-256, формирование ключа PBKDF2) в файле `~/.zonee/config`. Каждая последующая команда будет запрашивать PIN-код.

> API-ключи создаются в [настройках аккаунта Zone.eu](https://my.zone.eu) в разделе Безопасность > API-ключи.

**Переменные окружения (для CI/CD, Docker, скриптов):**

```bash
export ZONE_USER=your-username
export ZONE_APIKEY=your-api-key
```

Переменные окружения имеют приоритет над файлом конфигурации и не требуют PIN-кода.

Для использования зашифрованной конфигурации без интерактивного ввода PIN-кода:

```bash
export ZONE_PIN=your-pin
```

**Удаление сохранённых учётных данных:**

```bash
zonee auth logout
```

### Команды DNS

```bash
# Показать ВСЕ записи домена
zonee dns list example.com

# Показать записи определённого типа
zonee dns list example.com a
zonee dns list example.com mx
zonee dns list example.com txt

# Создать запись
zonee dns add example.com a www.example.com 1.2.3.4

# Обновить запись по ID
zonee dns update example.com a 12345 2.3.4.5

# Удалить запись по ID
zonee dns delete example.com a 12345

# Предпросмотр: что будет отправлено (без реального запроса)
zonee dns add example.com a www.example.com 1.2.3.4 --dry-run

# Вывод в формате JSON
zonee dns list example.com --json
```

Поддерживаемые типы записей: `a`, `aaaa`, `cname`, `mx`, `txt`, `srv`, `ns`, `caa`, `sshfp`, `url`, `tlsa`.

### Команды для доменов

```bash
# Показать все домены (с цветовой индикацией срока действия)
zonee domain list

# Фильтр по имени
zonee domain list --filter example

# Показать только домены, истекающие в течение 30 дней
zonee domain list --expiring

# Показать подробности о домене
zonee domain info example.com

# Продлить домен на 1 год
zonee domain renew example.com

# Продлить на 2 года
zonee domain renew example.com --period 2
```

### Общие параметры

| Параметр | Описание |
|---|---|
| `--json` | Вывод в формате JSON вместо таблицы |
| `--dry-run` | Показать, что будет отправлено, без выполнения запроса |
| `--version` | Показать версию |
| `--help` | Показать справку |

---

## Библиотека (Lfmt.Zonee)

```
dotnet add package Lfmt.Zonee
```

### Быстрый старт

```csharp
using ZoneEe;
using ZoneEe.Models;

using var client = new ZoneClient("username", "api-key");

// Показать все домены
var domains = await client.Domains.ListAsync();
foreach (var d in domains)
    Console.WriteLine($"{d.Name} expires {d.Expires}");

// Получить подробности о домене
var domain = await client.Domains.GetAsync("example.com");

// Показать все DNS-записи типа A
var records = await client.Dns.ListAsync("example.com", DnsRecordType.A);

// Создать DNS-запись
var created = await client.Dns.CreateAsync("example.com", DnsRecordType.A, new DnsRecordCreate
{
    Name = "www.example.com",
    Destination = "1.2.3.4"
});

// Создать MX-запись с приоритетом
await client.Dns.CreateAsync("example.com", DnsRecordType.MX, new DnsRecordCreate
{
    Name = "example.com",
    Destination = "mail.example.com",
    Priority = 10
});

// Обновить запись
await client.Dns.UpdateAsync("example.com", DnsRecordType.A, "12345", new DnsRecordUpdate
{
    Destination = "5.6.7.8"
});

// Удалить запись
await client.Dns.DeleteAsync("example.com", DnsRecordType.A, "12345");

// Продлить домен
await client.Domains.RenewAsync(new DomainRenew { Domain = "example.com", Period = 1 });
```

### Параметры конфигурации

```csharp
// Использование объекта параметров
var client = new ZoneClient(new ZoneClientOptions
{
    Username = "your-username",
    ApiKey = "your-api-key",
    BaseUrl = "https://api.zone.eu/v2"  // по умолчанию
});

// Использование внешнего HttpClient (для внедрения зависимостей / тестирования)
var httpClient = new HttpClient { BaseAddress = new Uri("https://api.zone.eu/v2/") };
var client = new ZoneClient(httpClient);
```

### Встроенные возможности

- **Обработка лимита запросов** -- автоматический повтор при HTTP 429 (до 3 раз с нарастающей задержкой)
- **Автопагинация** -- методы `ListAsync` прозрачно загружают все страницы
- **Подробные ошибки** -- `ZoneApiException` содержит код состояния, тело ответа и сообщение Zone.eu

---

## Типичные сценарии

### Подключение домена к Azure App Service

```bash
# Подтверждение владения доменом
zonee dns add example.com txt asuid.www.example.com "<verification-id>"

# Направить www на Azure
zonee dns add example.com cname www.example.com myapp.azurewebsites.net

# Направить корневой домен на IP Azure
zonee dns add example.com a example.com <azure-ip>
zonee dns add example.com txt asuid.example.com "<verification-id>"
```

### Подключение домена к Azure Static Web Apps

```bash
zonee dns add example.com cname www.example.com <app>.azurestaticapps.net
zonee dns add example.com txt example.com "<validation-token>"
zonee dns add example.com a example.com <azure-static-ip>
```

### Подключение домена к Vercel

```bash
zonee dns add example.com a example.com 76.76.21.21
zonee dns add example.com cname www.example.com cname.vercel-dns.com
```

### DNS-01 challenge для Let's Encrypt

```bash
zonee dns add example.com txt _acme-challenge.example.com "<token>"
# После получения сертификата:
zonee dns delete example.com txt <record-id>
```

### Использование с Claude Code

Скопируйте [`docs/CLAUDE-DNS-TEMPLATE.md`](docs/CLAUDE-DNS-TEMPLATE.md) в ваш проект как `CLAUDE.md` (или добавьте к существующему), чтобы Claude Code знала как управлять DNS для вашего домена.

---

## Сборка из исходного кода

```bash
git clone https://github.com/Lifemotion/zonee.git
cd zonee
dotnet build
```

**Публикация нативного AOT-бинарника:**

```bash
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r win-x64
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r linux-x64
dotnet publish src/ZoneEe.Cli/ZoneEe.Cli.csproj -c Release -r linux-arm64
```

## Лицензия

MIT -- Sasha Sovenko
