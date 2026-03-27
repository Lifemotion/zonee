using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;
using ZoneEe.Models;

namespace ZoneEe.Cli.Commands.Domain;

internal class DomainRegisterSettings : SharedSettings
{
    [CommandArgument(0, "<domain>")]
    [Description("Domain name to register (e.g. example.com)")]
    public string Domain { get; set; } = "";

    [CommandOption("-p|--period")]
    [Description("Registration period in years (default: 1)")]
    [DefaultValue(1)]
    public int Period { get; set; } = 1;
}

internal class DomainRegisterCommand : AsyncCommand<DomainRegisterSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DomainRegisterSettings settings)
    {
        var request = new DomainRegister
        {
            Name = settings.Domain,
            Period = settings.Period
        };

        if (settings.DryRun)
        {
            AnsiConsole.MarkupLine("[yellow]DRY RUN[/] — would register:");
            OutputFormatter.WriteJson(request);
            return 0;
        }

        using var client = new ZoneClient(AuthProvider.Resolve());
        var result = await client.Domains.RegisterAsync(request);

        if (settings.Json)
        {
            OutputFormatter.WriteJson(result);
            return 0;
        }

        AnsiConsole.MarkupLine($"[green]Registered[/] {result?.Name} (status: {result?.Status})");
        return 0;
    }
}
