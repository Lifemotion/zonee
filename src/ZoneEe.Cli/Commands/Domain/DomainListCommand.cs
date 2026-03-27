using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;

namespace ZoneEe.Cli.Commands.Domain;

internal class DomainListSettings : SharedSettings
{
}

internal class DomainListCommand : AsyncCommand<DomainListSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DomainListSettings settings)
    {
        using var client = new ZoneClient(AuthProvider.Resolve());
        var domains = await client.Domains.ListAsync();

        if (settings.Json)
        {
            OutputFormatter.WriteJson(domains);
            return 0;
        }

        var table = OutputFormatter.CreateTable("Name", "Expires", "AutoRenew", "DNSSEC");
        foreach (var d in domains)
            table.AddRow(d.Name, d.Expires ?? "-", d.AutoRenew ? "yes" : "no", d.Dnssec ? "yes" : "no");

        OutputFormatter.WriteTable(table);
        return 0;
    }
}
