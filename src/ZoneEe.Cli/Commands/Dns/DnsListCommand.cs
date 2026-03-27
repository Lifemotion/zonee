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

    [CommandArgument(1, "<type>")]
    [Description("Record type (a, aaaa, cname, mx, txt, srv, ns, soa)")]
    public string RecordType { get; set; } = "";
}

internal class DnsListCommand : AsyncCommand<DnsListSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DnsListSettings settings)
    {
        if (!Enum.TryParse<DnsRecordType>(settings.RecordType, ignoreCase: true, out var type))
        {
            Console.Error.WriteLine($"Unknown record type: {settings.RecordType}");
            return 1;
        }

        using var client = new ZoneClient(AuthProvider.Resolve());
        var records = await client.Dns.ListAsync(settings.Domain, type);

        if (settings.Json)
        {
            OutputFormatter.WriteJson(records);
            return 0;
        }

        var table = OutputFormatter.CreateTable("ID", "Name", "Destination");
        foreach (var r in records)
            table.AddRow(r.Id.ToString(), r.Name, r.Destination);

        OutputFormatter.WriteTable(table);
        return 0;
    }
}
