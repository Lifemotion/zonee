using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Commands.Auth;
using ZoneEe.Cli.Commands.Dns;
using ZoneEe.Cli.Commands.Domain;

if (args is ["--version" or "-v"])
{
    var version = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? "dev";
    AnsiConsole.WriteLine($"zonee {version}");
    AnsiConsole.WriteLine("https://github.com/Lifemotion/zonee");
    return 0;
}

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("zonee");
    config.SetApplicationVersion(
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? "dev");

    config.AddBranch("auth", auth =>
    {
        auth.SetDescription("Manage credentials");

        auth.AddCommand<AuthLoginCommand>("login")
            .WithDescription("Set up encrypted credentials")
            .WithExample("auth", "login");

        auth.AddCommand<AuthLogoutCommand>("logout")
            .WithDescription("Remove saved credentials")
            .WithExample("auth", "logout");
    });

    config.AddBranch("dns", dns =>
    {
        dns.SetDescription("Manage DNS records");

        dns.AddCommand<DnsListCommand>("list")
            .WithDescription("List DNS records (all types if type omitted)")
            .WithExample("dns", "list", "example.com")
            .WithExample("dns", "list", "example.com", "a");

        dns.AddCommand<DnsAddCommand>("add")
            .WithDescription("Create a DNS record")
            .WithExample("dns", "add", "example.com", "a", "www.example.com", "1.2.3.4");

        dns.AddCommand<DnsUpdateCommand>("update")
            .WithDescription("Update a DNS record")
            .WithExample("dns", "update", "example.com", "a", "12345", "2.3.4.5");

        dns.AddCommand<DnsDeleteCommand>("delete")
            .WithDescription("Delete a DNS record")
            .WithExample("dns", "delete", "example.com", "a", "12345");
    });

    config.AddBranch("domain", domain =>
    {
        domain.SetDescription("Manage domains");

        domain.AddCommand<DomainListCommand>("list")
            .WithDescription("List all domains")
            .WithExample("domain", "list");

        domain.AddCommand<DomainInfoCommand>("info")
            .WithDescription("Show domain details")
            .WithExample("domain", "info", "example.com");

        domain.AddCommand<DomainRenewCommand>("renew")
            .WithDescription("Renew or reactivate a domain")
            .WithExample("domain", "renew", "example.com");
    });
});

var result = app.Run(args);

if (args.Length == 0)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[yellow]AUTH:[/]");
    AnsiConsole.MarkupLine("    zonee auth login                     Interactive setup (credentials encrypted with PIN)");
    AnsiConsole.MarkupLine("    [dim]export[/] ZONE_USER=... ZONE_APIKEY=...  Environment variables (CI/CD, Docker)");
    AnsiConsole.MarkupLine("    [dim]export[/] ZONE_PIN=...                   Skip interactive PIN prompt (scripts, Claude Code)");
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("Docs & source: [blue underline]https://github.com/Lifemotion/zonee[/]");
}

return result;
