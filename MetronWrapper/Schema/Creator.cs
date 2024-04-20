using System.Text.Json.Serialization;

namespace MetronWrapper.Schema;

public record Creator : BaseResource
{
    public DateTime? Birth { get; init; } = null;
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    public DateTime? Death { get; init; } = null;
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Image { get; init; } = null;
    public List<string> Alias { get; init; } = [];
    public required string ResourceUrl { get; init; }
}