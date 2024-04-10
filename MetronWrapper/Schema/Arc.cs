using System.Text.Json.Serialization;

namespace MetronWrapper.Schema;

public record Arc : BaseResource
{
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Image { get; init; } = null;
    public required string ResourceUrl { get; init; }
}