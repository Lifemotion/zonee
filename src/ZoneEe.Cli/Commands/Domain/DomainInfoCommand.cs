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
        table.AddRow("Name", domain.Name);
        table.AddRow("Expires", domain.Expires ?? "-");
        table.AddRow("Expired", domain.Expired ? "yes" : "no");
        table.AddRow("AutoRenew", domain.AutoRenew ? "yes" : "no");
        table.AddRow("DNSSEC", domain.Dnssec ? "yes" : "no");
        table.AddRow("Custom NS", domain.NameserversCustom ? "yes" : "no");
        table.AddRow("Auth Key", domain.AuthKeyEnabled ? "yes" : "no");

        OutputFormatter.WriteTable(table);
        return 0;
    }
}
