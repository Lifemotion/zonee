using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;
using ZoneEe.Models;

namespace ZoneEe.Cli.Commands.Domain;

internal class DomainRenewSettings : SharedSettings
{
    [CommandArgument(0, "<domain>")]
    [Description("Domain name to renew (e.g. example.com)")]
    public string Domain { get; set; } = "";

    [CommandOption("-p|--period")]
    [Description("Renewal period in years (default: 1)")]
    [DefaultValue(1)]
    public int Period { get; set; } = 1;
}

internal class DomainRenewCommand : AsyncCommand<DomainRenewSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DomainRenewSettings settings)
    {
        var request = new DomainRenew
        {
            Domain = settings.Domain,
            Period = settings.Period
        };

        if (settings.DryRun)
        {
            AnsiConsole.MarkupLine("[yellow]DRY RUN[/] — would renew:");
            OutputFormatter.WriteJson(request);
            return 0;
        }

        using var client = new ZoneClient(AuthProvider.Resolve());
        await client.Domains.RenewAsync(request);

        AnsiConsole.MarkupLine($"[green]Renewed[/] {settings.Domain} for {settings.Period} year(s)");
        return 0;
    }
}
