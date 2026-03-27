using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;
using ZoneEe.Models;

namespace ZoneEe.Cli.Commands.Dns;

internal class DnsListSettings : SharedSettings
{
    [CommandArgument(0, "<domain>")]
    [Description("Domain name (e.g. example.com)")]
    public string Domain { get; set; } = "";

    [CommandArgument(1, "[type]")]
    [Description("Record type (a, aaaa, cname, mx, txt, srv, ns, caa, sshfp, url, tlsa). Omit to show all")]
    public string? RecordType { get; set; }
}

internal class DnsListCommand : AsyncCommand<DnsListSettings>
{
    private static readonly DnsRecordType[] AllTypes =
        Enum.GetValues<DnsRecordType>();

    public override async Task<int> ExecuteAsync(CommandContext context, DnsListSettings settings)
    {
        using var client = new ZoneClient(AuthProvider.Resolve());

        List<(DnsRecordType Type, DnsRecord Record)> allRecords;

        if (!string.IsNullOrEmpty(settings.RecordType))
        {
            if (!Enum.TryParse<DnsRecordType>(settings.RecordType, ignoreCase: true, out var type))
            {
                Console.Error.WriteLine($"Unknown record type: {settings.RecordType}");
                return 1;
            }

            var records = await client.Dns.ListAsync(settings.Domain, type);
            allRecords = records.Select(r => (type, r)).ToList();
        }
        else
        {
            var tasks = AllTypes.Select(async t =>
            {
                var records = await client.Dns.ListAsync(settings.Domain, t);
                return records.Select(r => (Type: t, Record: r));
            });

            var results = await Task.WhenAll(tasks);
            allRecords = results.SelectMany(r => r).ToList();
        }

        if (settings.Json)
        {
            OutputFormatter.WriteJson(allRecords.Select(r => r.Record).ToList());
            return 0;
        }

        if (allRecords.Count == 0)
        {
            AnsiConsole.MarkupLine("[dim]No records found[/]");
            return 0;
        }

        var table = OutputFormatter.CreateTable("Type", "ID", "Name", "Destination", "Extra");
        foreach (var (type, r) in allRecords)
        {
            var extra = FormatExtra(type, r);
            table.AddRow(
                type.ToString().ToUpperInvariant(),
                r.Id,
                r.Name,
                Markup.Escape(r.Destination),
                extra);
        }

        OutputFormatter.WriteTable(table);
        return 0;
    }

    private static string FormatExtra(DnsRecordType type, DnsRecord r) => type switch
    {
        DnsRecordType.MX => r.Priority is not null ? $"pri={r.Priority}" : "",
        DnsRecordType.SRV => string.Join(" ",
            new[] { r.Priority is not null ? $"pri={r.Priority}" : null,
                    r.Weight is not null ? $"w={r.Weight}" : null,
                    r.Port is not null ? $"port={r.Port}" : null }
            .Where(s => s is not null)),
        DnsRecordType.CAA => string.Join(" ",
            new[] { r.Flag is not null ? $"flag={r.Flag}" : null,
                    r.Tag is not null ? $"tag={r.Tag}" : null }
            .Where(s => s is not null)),
        _ => ""
    };
}
