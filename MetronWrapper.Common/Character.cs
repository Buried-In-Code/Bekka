using System.Text.Json.Serialization;

namespace MetronWrapper.Common;

public record Character : BaseResource
{
    public List<string> alias { get; init; } = new();
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    public List<BaseResource> Creators { get; init; } = new();
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Image { get; init; } = null;
    public string ResourceUrl { get; init; }
    public List<BaseResource> Teams { get; init; } = new();
    public List<BaseResource> Universes { get; init; } = new();
}