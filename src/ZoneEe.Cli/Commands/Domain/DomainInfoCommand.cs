using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;

namespace ZoneEe.Cli.Commands.Domain;

internal class DomainInfoSettings : SharedSettings
{
    [CommandArgument(0, "<domain>")]
    [Description("Domain name (e.g. example.com)")]
    public string Domain { get; set; } = "";
}

internal class DomainInfoCommand : AsyncCommand<DomainInfoSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DomainInfoSettings settings)
    {
        using var client = new ZoneClient(AuthProvider.Resolve());
        var domain = await client.Domains.GetAsync(settings.Domain);

        if (domain is null)
        {
            Console.Error.WriteLine($"Domain not found: {settings.Domain}");
            return 1;
        }

        if (settings.Json)
        {
            OutputFormatter.WriteJson(domain);
            return 0;
        }

        var table = OutputFormatter.CreateTable("Property", "Value");
        table.AddRow("Name", Markup.Escape(domain.Name));
        table.AddRow("Expires", DateFormatter.Format(domain.Expires));
        table.AddRow("AutoRenew", domain.AutoRenew ? "[green]yes[/]" : "[dim]no[/]");
        table.AddRow("DNSSEC", domain.Dnssec ? "[green]yes[/]" : "[dim]no[/]");
        table.AddRow("Custom NS", domain.NameserversCustom ? "yes" : "[dim]no[/]");
        table.AddRow("Auth Key", domain.AuthKeyEnabled ? "yes" : "[dim]no[/]");
        if (domain.Delegated is not null)
            table.AddRow("Delegated to", Markup.Escape(domain.Delegated));

        OutputFormatter.WriteTable(table);
        return 0;
    }
}
