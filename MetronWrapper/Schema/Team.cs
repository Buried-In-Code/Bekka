using System.Text.Json.Serialization;

namespace MetronWrapper.Schema;

public record Team : BaseResource
{
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    public List<BaseResource> Creators { get; init; } = [];
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Image { get; init; } = null;
    public required string ResourceUrl { get; init; }
    public List<BaseResource> Universes { get; init; } = [];
}