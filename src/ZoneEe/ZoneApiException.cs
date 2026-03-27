using System.Net;

namespace ZoneEe;

/// <summary>
/// Exception thrown when the Zone.eu API returns an error response.
/// </summary>
public class ZoneApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseBody { get; }
    public string? StatusMessage { get; }

    public ZoneApiException(HttpStatusCode statusCode, string? responseBody, string? statusMessage)
        : base(FormatMessage(statusCode, statusMessage, responseBody))
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        StatusMessage = statusMessage;
    }

    private static string FormatMessage(HttpStatusCode code, string? statusMessage, string? body)
    {
        var msg = $"Zone API error {(int)code} ({code})";
        if (!string.IsNullOrEmpty(statusMessage))
            msg += $": {statusMessage}";
        else if (!string.IsNullOrEmpty(body))
            msg += $": {body}";
        return msg;
    }
}
