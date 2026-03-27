using Spectre.Console;
using ZoneEe;

namespace ZoneEe.Cli.Infrastructure;

internal static class AuthProvider
{
    public static ZoneClientOptions Resolve()
    {
        // Environment variables take priority (for CI/CD, Docker, scripts)
        var user = Environment.GetEnvironmentVariable("ZONE_USER");
        var key = Environment.GetEnvironmentVariable("ZONE_APIKEY");

        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(key))
            return new ZoneClientOptions { Username = user, ApiKey = key };

        // Fallback to encrypted config file
        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".zonee", "config");

        if (!File.Exists(configPath))
            throw new InvalidOperationException(
                "Credentials not found. Run 'zonee auth login' to set up, " +
                "or set ZONE_USER and ZONE_APIKEY environment variables.");

        var encrypted = File.ReadAllBytes(configPath);

        // PIN from env (for scripts) or interactive prompt
        var pin = Environment.GetEnvironmentVariable("ZONE_PIN");
        if (string.IsNullOrEmpty(pin))
            pin = AnsiConsole.Prompt(new TextPrompt<string>("PIN:").Secret());

        var plaintext = CryptoHelper.Decrypt(encrypted, pin);
        if (plaintext is null)
            throw new InvalidOperationException("Wrong PIN.");

        foreach (var line in plaintext.Split('\n'))
        {
            var trimmed = line.Trim();
            if (!trimmed.Contains('='))
                continue;

            var sep = trimmed.IndexOf('=');
            var k = trimmed[..sep].Trim().ToLowerInvariant();
            var v = trimmed[(sep + 1)..].Trim();

            switch (k)
            {
                case "username": user = v; break;
                case "apikey": key = v; break;
            }
        }

        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(key))
            return new ZoneClientOptions { Username = user, ApiKey = key };

        throw new InvalidOperationException("Config file is corrupted. Run 'zonee auth login' again.");
    }
}
