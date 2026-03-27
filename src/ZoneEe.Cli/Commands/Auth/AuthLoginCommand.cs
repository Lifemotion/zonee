using Spectre.Console;
using Spectre.Console.Cli;
using ZoneEe.Cli.Infrastructure;

namespace ZoneEe.Cli.Commands.Auth;

internal class AuthLoginCommand : Command
{
    public override int Execute(CommandContext context)
    {
        AnsiConsole.MarkupLine("[bold]Zone.eu API credentials setup[/]");
        AnsiConsole.WriteLine();

        var username = AnsiConsole.Ask<string>("Username:");
        var apiKey = AnsiConsole.Prompt(
            new TextPrompt<string>("API key:").Secret());
        var pin = AnsiConsole.Prompt(
            new TextPrompt<string>("PIN (for encrypting credentials):").Secret());
        var pinConfirm = AnsiConsole.Prompt(
            new TextPrompt<string>("Confirm PIN:").Secret());

        if (pin != pinConfirm)
        {
            AnsiConsole.MarkupLine("[red]PINs do not match.[/]");
            return 1;
        }

        if (string.IsNullOrWhiteSpace(pin))
        {
            AnsiConsole.MarkupLine("[red]PIN cannot be empty.[/]");
            return 1;
        }

        var configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".zonee");

        Directory.CreateDirectory(configDir);

        var plaintext = $"username={username}\napikey={apiKey}";
        var encrypted = CryptoHelper.Encrypt(plaintext, pin);
        File.WriteAllBytes(Path.Combine(configDir, "config"), encrypted);

        AnsiConsole.MarkupLine($"[green]Credentials saved to {configDir}/config (encrypted)[/]");
        return 0;
    }
}
