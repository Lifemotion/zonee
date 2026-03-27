using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

namespace ZoneEe.Services;

internal static class PaginationHelper
{
    /// <summary>
    /// Fetches all pages from a paginated Zone.eu API endpoint.
    /// Checks x-pager-enabled header and iterates through x-pager-pages.
    /// </summary>
    public static async Task<List<T>> GetAllPagesAsync<T>(
        HttpClient http,
        string path,
        JsonTypeInfo<List<T>> typeInfo,
        CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Add("x-pager-limit", "100");
        request.Headers.Add("x-pager-page", "1");

        var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync(typeInfo, ct) ?? [];

        // Check if pagination is enabled
        if (!response.Headers.TryGetValues("x-pager-enabled", out var pagerEnabled)
            || pagerEnabled.FirstOrDefault() != "1")
            return items;

        // Get total pages
        if (!response.Headers.TryGetValues("x-pager-pages", out var pagesHeader)
            || !int.TryParse(pagesHeader.FirstOrDefault(), out var totalPages)
            || totalPages <= 1)
            return items;

        // Fetch remaining pages
        for (var page = 2; page <= totalPages; page++)
        {
            var pageRequest = new HttpRequestMessage(HttpMethod.Get, path);
            pageRequest.Headers.Add("x-pager-limit", "100");
            pageRequest.Headers.Add("x-pager-page", page.ToString());

            var pageResponse = await http.SendAsync(pageRequest, ct);
            pageResponse.EnsureSuccessStatusCode();

            var pageItems = await pageResponse.Content.ReadFromJsonAsync(typeInfo, ct);
            if (pageItems is { Count: > 0 })
                items.AddRange(pageItems);
        }

        return items;
    }
}
