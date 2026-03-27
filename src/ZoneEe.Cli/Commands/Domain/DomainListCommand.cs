using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;

namespace ZoneEe.Cli.Commands.Domain;

internal class DomainListSettings : SharedSettings
{
    [CommandOption("--expiring")]
    [Description("Show only domains expiring within 30 days")]
    public bool Expiring { get; set; }

    [CommandOption("-f|--filter <NAME>")]
    [Description("Filter domains by name")]
    public string? Filter { get; set; }
}

internal class DomainListCommand : AsyncCommand<DomainListSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DomainListSettings settings)
    {
        using var client = new ZoneClient(AuthProvider.Resolve());
        var domains = await client.Domains.ListAsync();

        if (!string.IsNullOrEmpty(settings.Filter))
            domains = domains.Where(d => d.Name.Contains(settings.Filter, StringComparison.OrdinalIgnoreCase)).ToList();

        if (settings.Expiring)
        {
            var cutoff = DateTimeOffset.Now.AddDays(30);
            domains = domains.Where(d =>
                DateTimeOffset.TryParse(d.Expires, out var exp) && exp <= cutoff).ToList();
        }

        if (settings.Json)
        {
            OutputFormatter.WriteJson(domains);
            return 0;
        }

        if (domains.Count == 0)
        {
            AnsiConsole.MarkupLine("[dim]No domains found[/]");
            return 0;
        }

        var table = OutputFormatter.CreateTable("Name", "Expires", "AutoRenew", "DNSSEC");
        foreach (var d in domains)
            table.AddRow(
                Markup.Escape(d.Name),
                DateFormatter.Format(d.Expires),
                d.AutoRenew ? "[green]yes[/]" : "[dim]no[/]",
                d.Dnssec ? "[green]yes[/]" : "[dim]no[/]");

        OutputFormatter.WriteTable(table);
        return 0;
    }
}
