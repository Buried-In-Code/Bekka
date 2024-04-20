namespace MetronWrapper.Test.Schema;

[TestFixture]
public class UniverseTest
{
    private Metron session;

    [OneTimeSetUp]
    public void BeforeTests()
    {
        var username = Environment.GetEnvironmentVariable("METRON__USERNAME") ?? "IGNORED";
        var password = Environment.GetEnvironmentVariable("METRON__PASSWORD") ?? "IGNORED";
        var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        var cache = new SQLiteCache(path: Path.Combine(projectDirectory, "cache.sqlite"), expiry: null);
        session = new Metron(username: username, password: password, cache: cache);
    }

    [Test(Description = "Test ListUniverses with a valid search")]
    public async Task TestListUniverses()
    {
        var results = await session.ListUniverses(parameters: new Dictionary<string, string> { { "name", "Earth 2" } });
        Assert.That(results, Has.Count.EqualTo(5));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(18));
            Assert.That(results[0].Name, Is.EqualTo("Earth 2"));
        });
    }

    [Test(Description = "Test ListUniverses with an invalid search")]
    public async Task TestListUniversesFail()
    {
        var results = await session.ListUniverses(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetUniverse with a valid id")]
    public async Task TestGetUniverse()
    {
        var result = await session.GetUniverse(id: 18);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Designation, Is.EqualTo("Earth 2"));
            Assert.That(result.Id, Is.EqualTo(18));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/universe/2024/01/25/earth-2.webp"));
            Assert.That(result.Name, Is.EqualTo("Earth 2"));
            Assert.That(result.Publisher.Id, Is.EqualTo(2));
            Assert.That(result.Publisher.Name, Is.EqualTo("DC Comics"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/universe/earth-2/"));
        });
    }

    [Test(Description = "Test GetUniverse with an invalid id")]
    public void TestGetUniverseFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetUniverse(id: -1));
    }
}