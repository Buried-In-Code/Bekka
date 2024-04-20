using System.Text.Json.Serialization;

namespace Bekka.Schema;

public record Credit
{
    public required string Creator { get; init; }
    public required long Id { get; init; }
    public List<GenericItem> Role { get; init; } = [];
}

public record Reprint
{
    public required long Id { get; init; }
    [JsonPropertyName("issue")]
    public required string Name { get; init; }
}

public record BasicSeries
{
    public required string Name { get; init; }
    public required int Volume { get; init; }
    public required int YearBegan { get; init; }
}

public record IssueSeries : GenericItem
{
    public List<GenericItem> Genres { get; init; } = [];
    public required GenericItem SeriesType { get; init; }
    public required string SortName { get; init; }
    public required int Volume { get; init; }
}

public record Variant
{
    public required string Image { get; init; }
    public string? Name { get; init; } = null;
    public string? Sku { get; init; } = null;
    public string? Upc { get; init; } = null;
}

public record BaseIssue
{
    public required DateTime CoverDate { get; init; }
    public string? CoverHash { get; init; } = null;
    public required long Id { get; init; }
    public string? Image { get; init; } = null;
    public required DateTime Modified { get; init; }
    public required string Number { get; init; }
    public DateTime? StoreDate { get; init; } = null;
}

public record CommonIssue : BaseIssue
{
    [JsonPropertyName("issue")]
    public required string Name { get; init; }
    public required BasicSeries Series { get; init; }
}

public record Issue : BaseIssue
{
    public List<BaseResource> Arcs { get; init; } = [];
    public List<BaseResource> Characters { get; init; } = [];
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    public List<Credit> Credits { get; init; } = [];
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Isbn { get; init; } = null;
    [JsonPropertyName("page")]
    public int? PageCount { get; init; } = null;
    public decimal? Price { get; init; } = null;
    public required GenericItem Publisher { get; init; }
    public required GenericItem Rating { get; init; }
    public List<Reprint> Reprints { get; init; } = [];
    public required string ResourceUrl { get; init; }
    public required IssueSeries Series { get; init; }
    public string? Sku { get; init; } = null;
    [JsonPropertyName("name")]
    public List<string> Stories { get; init; } = [];
    public List<BaseResource> Teams { get; init; } = [];
    public string? Title { get; init; } = null;
    public List<BaseResource> Universes { get; init; } = [];
    public string? Upc { get; init; } = null;
    public List<Variant> Variants { get; init; } = [];
}