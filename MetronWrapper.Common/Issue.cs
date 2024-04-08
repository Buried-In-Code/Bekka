using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetronWrapper.Common;

public record Credit
{
    public string Creator { get; init; }
    public int Id { get; init; }
    public List<GenericItem> Role { get; init; } = new();
}

public record Reprint
{
    public int Id { get; init; }
    [JsonPropertyName("issue")]
    public string Name { get; init; }
}

public record BasicSeries
{
    public string Name { get; init; }
    public int Volume { get; init; }
    public int YearBegan { get; init; }
}

public record IssueSeries
{
    public List<GenericItem> Genres { get; init; } = new();
    public long Id { get; init; }
    public string Name { get; init; }
    public GenericItem SeriesType { get; init; }
    public string SortName { get; init; }
    public int Volume { get; init; }
}

public record BaseIssue
{
    public DateTime CoverDate { get; init; }
    public string? CoverHash { get; init; } = null;
    public long Id { get; init; }
    public string? Image { get; init; } = null;
    public DateTime Modified { get; init; }
    public string Number { get; init; }
    public DateTime? StoreDate { get; init; } = null;
}

public record Variant
{
    public string Image { get; init; }
    public string? Name { get; init; } = null;
    public string? Sku { get; init; } = null;
    public string? Upc { get; init; } = null;
}

public record CommonIssue : BaseIssue
{
    [JsonPropertyName("issue")]
    public string Name { get; init; }
    public BasicSeries Series { get; init; }
}

public record Issue : BaseIssue
{
    public List<BaseResource> Arcs { get; init; } = new();
    public List<BaseResource> Characters { get; init; } = new();
    [JsonPropertyName("cv_id")]
    public long? ComicvineId { get; init; } = null;
    public List<Credit> Credits { get; init; } = new();
    [JsonPropertyName("desc")]
    public string? Description { get; init; } = null;
    public string? Isbn { get; init; } = null;
    [JsonPropertyName("page")]
    public int? PageCount { get; init; } = null;
    public decimal? Price { get; init; } = null;
    public GenericItem Publisher { get; init; }
    public GenericItem Rating { get; init; }
    public List<Reprint> Reprints { get; init; } = new();
    public string ResourceUrl { get; init; }
    public IssueSeries Series { get; init; }
    public string? Sku { get; init; } = null;
    [JsonPropertyName("name")]
    public List<string> Stories { get; init; } = new();
    public List<BaseResource> Teams { get; init; } = new();
    public string Title { get; init; }
    public List<BaseResource> Universes { get; init; } = new();
    public string? Upc { get; init; } = null;
    public List<Variant> Variants { get; init; } = new();
}