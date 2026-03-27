using Spectre.Console;
using Spectre.Console.Cli;

namespace ZoneEe.Cli.Commands.Auth;

internal class AuthLogoutCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".zonee", "config");

        if (!File.Exists(configPath))
        {
            AnsiConsole.MarkupLine("[yellow]No saved credentials found.[/]");
            return 0;
        }

        File.Delete(configPath);
        AnsiConsole.MarkupLine("[green]Credentials removed.[/]");
        return 0;
    }
}
