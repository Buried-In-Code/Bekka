namespace MetronWrapper.Test;

[TestFixture]
public class TestUniverse
{
    private Metron _metron;

    [OneTimeSetUp]
    public void BeforeTests()
    {
        var username = Environment.GetEnvironmentVariable("METRON__USERNAME") ?? "IGNORED";
        var password = Environment.GetEnvironmentVariable("METRON__PASSWORD") ?? "IGNORED";
        var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        var cache = new SQLiteCache(path: Path.Combine(projectDirectory, "cache.sqlite"), expiry: null);
        _metron = new Metron(username: username, password: password, cache: cache);
    }

    [Test(Description = "Test using the GetUniverse function with a valid Id")]
    public async Task TestGetUniverse()
    {
        var result = await _metron.GetUniverse(id: 18);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(18));
            Assert.That(result.Name, Is.EqualTo("Earth 2"));
            Assert.That(result.Designation, Is.EqualTo("Earth 2"));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/universe/2024/01/25/earth-2.webp"));
            Assert.That(result.Publisher.Id, Is.EqualTo(2));
            Assert.That(result.Publisher.Name, Is.EqualTo("DC Comics"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/universe/earth-2/"));
        });
    }

    [Test(Description = "Test using the GetUniverse function with an invalid Id")]
    public void TestGetUniverseFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetUniverse(id: -1));
    }

    [Test(Description = "Test using the ListUniverses function with a valid search")]
    public async Task TestListUniverses()
    {
        var results = await _metron.ListUniverses(parameters: new Dictionary<string, string> { { "name", "Earth 2" } });
        Assert.That(results, Has.Count.EqualTo(5));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(18));
            Assert.That(results[0].Name, Is.EqualTo("Earth 2"));
        });
    }

    [Test(Description = "Test using the ListUniverses function with an invalid search")]
    public async Task TestListUniversesFail()
    {
        var results = await _metron.ListUniverses(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }
}