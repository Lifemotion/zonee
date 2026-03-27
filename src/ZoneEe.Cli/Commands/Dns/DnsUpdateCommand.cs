using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;
using ZoneEe.Models;

namespace ZoneEe.Cli.Commands.Dns;

internal class DnsUpdateSettings : SharedSettings
{
    [CommandArgument(0, "<domain>")]
    [Description("Domain name")]
    public string Domain { get; set; } = "";

    [CommandArgument(1, "<type>")]
    [Description("Record type")]
    public string RecordType { get; set; } = "";

    [CommandArgument(2, "<id>")]
    [Description("Record ID")]
    public string Id { get; set; } = "";

    [CommandArgument(3, "<destination>")]
    [Description("New destination value")]
    public string Destination { get; set; } = "";
}

internal class DnsUpdateCommand : AsyncCommand<DnsUpdateSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DnsUpdateSettings settings)
    {
        if (!Enum.TryParse<DnsRecordType>(settings.RecordType, ignoreCase: true, out var type))
        {
            Console.Error.WriteLine($"Unknown record type: {settings.RecordType}");
            return 1;
        }

        var update = new DnsRecordUpdate { Destination = settings.Destination };

        if (settings.DryRun)
        {
            AnsiConsole.MarkupLine($"[yellow]DRY RUN[/] — would update record {settings.Id}:");
            OutputFormatter.WriteJson(update);
            return 0;
        }

        using var client = new ZoneClient(AuthProvider.Resolve());
        var updated = await client.Dns.UpdateAsync(settings.Domain, type, settings.Id, update);

        if (settings.Json)
        {
            OutputFormatter.WriteJson(updated);
            return 0;
        }

        AnsiConsole.MarkupLine($"[green]Updated[/] record {updated?.Id} → {updated?.Destination}");
        return 0;
    }
}
