using Spectre.Console.Cli;
using ZoneEe.Cli.Commands.Dns;
using ZoneEe.Cli.Commands.Domain;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("zonee");

    config.AddBranch("dns", dns =>
    {
        dns.SetDescription("Manage DNS records");

        dns.AddCommand<DnsListCommand>("list")
            .WithDescription("List DNS records")
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

        domain.AddCommand<DomainRegisterCommand>("register")
            .WithDescription("Register a new domain")
            .WithExample("domain", "register", "example.com");
    });
});

return app.Run(args);
