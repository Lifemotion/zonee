using Spectre.Console;

namespace ZoneEe.Cli.Infrastructure;

internal static class DateFormatter
{
    /// <summary>
    /// Formats an ISO date string as "2027-03-04 (342d)" with color coding.
    /// Red if expired, yellow if expiring within 30 days, green otherwise.
    /// </summary>
    public static string Format(string? isoDate)
    {
        if (string.IsNullOrEmpty(isoDate))
            return "[dim]-[/]";

        if (!DateTimeOffset.TryParse(isoDate, out var date))
            return Markup.Escape(isoDate);

        var now = DateTimeOffset.Now;
        var days = (int)(date - now).TotalDays;
        var short_date = date.ToString("yyyy-MM-dd");

        if (days < 0)
            return $"[red]{short_date} ({-days}d ago)[/]";
        if (days <= 30)
            return $"[yellow]{short_date} ({days}d)[/]";
        return $"[green]{short_date}[/] [dim]({days}d)[/]";
    }
}
