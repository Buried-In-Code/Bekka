namespace MetronWrapper.Schema;

public record ListResponse<T>
{
    public required int Count { get; init; }
    public string? Next { get; init; } = null;
    public string? Previous { get; init; } = null;
    public List<T> Results { get; init; } = [];
}

public record GenericItem
{
    public required long Id { get; init; }
    public required string Name { get; init; }
}

public record BaseResource : GenericItem
{
    public required DateTime Modified { get; init; }
}