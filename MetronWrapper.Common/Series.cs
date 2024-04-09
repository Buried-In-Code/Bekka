using System.Text.Json.Serialization;

namespace MetronWrapper.Common;

public record AssociatedSeries
{
    public int Id { get; init; }
    [JsonPropertyName("series")]
    public string Name { get; init; }
}

public record BaseSeries
{
    public long Id { get; init; }
    public int IssueCount { get; init; }
    public DateTime Modified { get; init; }
    public int Volume { get; init; }
    public int YearBegan { get; init; }
}

public record CommonSeries : BaseSeries
{
    [JsonPropertyName("series")]
    public string Name { get; init; }
}

public record Series : BaseSeries
{
    public List<AssociatedSeries> Associated { get; init; } = new();
    [JsonPropertyName("cv_id")]
    public long? Comicvine { get; init; } = null;
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public List<GenericItem> Genres { get; init; } = new();
    public string Name { get; init; }
    public GenericItem Publisher { get; init; }
    public string ResourceUrl { get; init; }
    public GenericItem SeriesType { get; init; }
    public string SortName { get; init; }
    public int? YearEnd { get; init; } = null;
}