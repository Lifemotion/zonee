using ZoneEe;

namespace ZoneEe.Cli.Infrastructure;

internal static class AuthProvider
{
    public static ZoneClientOptions Resolve()
    {
        // Environment variables take priority
        var user = Environment.GetEnvironmentVariable("ZONE_USER");
        var key = Environment.GetEnvironmentVariable("ZONE_APIKEY");

        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(key))
            return new ZoneClientOptions { Username = user, ApiKey = key };

        // Fallback to config file
        var configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".zonee", "config");

        if (File.Exists(configPath))
        {
            var lines = File.ReadAllLines(configPath);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith('#') || !trimmed.Contains('='))
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
        }

        throw new InvalidOperationException(
            "Credentials not found. Set ZONE_USER and ZONE_APIKEY environment variables, " +
            $"or create {configPath} with username= and apikey= lines.");
    }
}
