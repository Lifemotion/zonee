using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;
using ZoneEe.Models;

namespace ZoneEe.Cli.Commands.Dns;

internal class DnsAddSettings : SharedSettings
{
    [CommandArgument(0, "<domain>")]
    [Description("Domain name (e.g. example.com)")]
    public string Domain { get; set; } = "";

    [CommandArgument(1, "<type>")]
    [Description("Record type (a, aaaa, cname, mx, txt, srv, ns)")]
    public string RecordType { get; set; } = "";

    [CommandArgument(2, "<name>")]
    [Description("Record name (e.g. www.example.com or @ for root)")]
    public string Name { get; set; } = "";

    [CommandArgument(3, "<destination>")]
    [Description("Destination value (IP address, hostname, etc.)")]
    public string Destination { get; set; } = "";
}

internal class DnsAddCommand : AsyncCommand<DnsAddSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DnsAddSettings settings)
    {
        if (!Enum.TryParse<DnsRecordType>(settings.RecordType, ignoreCase: true, out var type))
        {
            Console.Error.WriteLine($"Unknown record type: {settings.RecordType}");
            return 1;
        }

        var name = settings.Name == "@" ? settings.Domain : settings.Name;

        var record = new DnsRecordCreate
        {
            Name = name,
            Destination = settings.Destination
        };

        if (settings.DryRun)
        {
            AnsiConsole.MarkupLine("[yellow]DRY RUN[/] — would create:");
            OutputFormatter.WriteJson(record);
            return 0;
        }

        using var client = new ZoneClient(AuthProvider.Resolve());
        var created = await client.Dns.CreateAsync(settings.Domain, type, record);

        if (settings.Json)
        {
            OutputFormatter.WriteJson(created);
            return 0;
        }

        AnsiConsole.MarkupLine($"[green]Created[/] record {created?.Id} ({type}) for {settings.Domain}");
        return 0;
    }
}
