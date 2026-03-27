using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Spectre.Console;

namespace ZoneEe.Cli.Infrastructure;

internal static class OutputFormatter
{
    public static void WriteJson<T>(T data)
    {
        var typeInfo = (JsonTypeInfo<T>)AppJsonContext.Default.GetTypeInfo(typeof(T))!;
        Console.WriteLine(JsonSerializer.Serialize(data, typeInfo));
    }

    public static void WriteTable(Table table)
    {
        AnsiConsole.Write(table);
    }

    public static Table CreateTable(params string[] columns)
    {
        var table = new Table().Border(TableBorder.Rounded);
        foreach (var col in columns)
            table.AddColumn(col);
        return table;
    }
}
