using System.Text.Json;

namespace TechMoveGLMS.Services.ApiClients;

internal static class JsonHelper
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options)!;
}
