using System.Text.Json.Serialization;

namespace Bekka.Schema;

public record Universe : BaseResource
{
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Designation { get; init; } = null;
    public string? Image { get; init; } = null;
    public required GenericItem Publisher { get; init; }
    public required string ResourceUrl { get; init; }
}