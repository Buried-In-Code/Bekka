using System.Text.Json.Serialization;

namespace MetronWrapper.Schema;

public record Character : BaseResource
{
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Image { get; init; } = null;
    public List<string> Alias { get; init; } = [];
    public List<BaseResource> Creators { get; init; } = [];
    public List<BaseResource> Teams { get; init; } = [];
    public List<BaseResource> Universes { get; init; } = [];
    public required string ResourceUrl { get; init; }
}