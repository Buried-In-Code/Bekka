using System.Text.Json.Serialization;

namespace Bekka.Schema;

public record AssociatedSeries
{
    public required long Id { get; init; }
    [JsonPropertyName("series")]
    public required string Name { get; init; }
}

public record BaseSeries
{
    public required long Id { get; init; }
    public required int IssueCount { get; init; }
    public required DateTime Modified { get; init; }
    public required int Volume { get; init; }
    public required int YearBegan { get; init; }
}

public record CommonSeries : BaseSeries
{
    [JsonPropertyName("series")]
    public required string Name { get; init; }
}

public record Series : BaseSeries
{
    public List<AssociatedSeries> Associated { get; init; } = [];
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public List<GenericItem> Genres { get; init; } = [];
    public required string Name { get; init; }
    public required GenericItem Publisher { get; init; }
    public required string ResourceUrl { get; init; }
    public required GenericItem SeriesType { get; init; }
    public required string SortName { get; init; }
    public int? YearEnd { get; init; } = null;
}