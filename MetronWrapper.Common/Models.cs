namespace MetronWrapper.Common;

public record ListResponse<T>
{
    public int Count { get; init; }
    public string? Next { get; init; } = null;
    public string? Previous { get; init; } = null;
    public List<T> Results { get; init; } = new();
}

public record BaseResource
{
    public long Id { get; init; }
    public DateTime Modified { get; init; }
    public string Name { get; init; }
}

public record GenericItem
{
    public long Id { get; init; }
    public string Name { get; init; }
}