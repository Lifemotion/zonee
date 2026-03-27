using System.ComponentModel;
using Spectre.Console.Cli;

namespace ZoneEe.Cli.Commands;

internal class SharedSettings : CommandSettings
{
    [CommandOption("--json")]
    [Description("Output as JSON instead of a table")]
    public bool Json { get; set; }

    [CommandOption("--dry-run")]
    [Description("Show what would be sent without making the request")]
    public bool DryRun { get; set; }
}
