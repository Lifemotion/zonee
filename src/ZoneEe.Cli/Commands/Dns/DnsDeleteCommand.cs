using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;
using ZoneEe.Models;

namespace ZoneEe.Cli.Commands.Dns;

internal class DnsDeleteSettings : SharedSettings
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
}

internal class DnsDeleteCommand : AsyncCommand<DnsDeleteSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DnsDeleteSettings settings)
    {
        if (!Enum.TryParse<DnsRecordType>(settings.RecordType, ignoreCase: true, out var type))
        {
            Console.Error.WriteLine($"Unknown record type: {settings.RecordType}");
            return 1;
        }

        if (settings.DryRun)
        {
            AnsiConsole.MarkupLine($"[yellow]DRY RUN[/] — would delete record {settings.Id} ({type}) from {settings.Domain}");
            return 0;
        }

        using var client = new ZoneClient(AuthProvider.Resolve());
        await client.Dns.DeleteAsync(settings.Domain, type, settings.Id);

        AnsiConsole.MarkupLine($"[green]Deleted[/] record {settings.Id} from {settings.Domain}");
        return 0;
    }
}
