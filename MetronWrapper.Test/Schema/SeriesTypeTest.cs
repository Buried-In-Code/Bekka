namespace MetronWrapper.Test.Schema;

[TestFixture]
public class SeriesTypeTest
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

    [Test(Description = "Test ListSeriesTypes with a valid search")]
    public async Task TestListSeriesTypes()
    {
        var results = await session.ListSeriesTypes(parameters: new Dictionary<string, string> { { "name", "Ongoing Series" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo("Ongoing Series"));
        });
    }

    [Test(Description = "Test ListSeriesTypes with an invalid search")]
    public async Task TestListSeriesTypesFail()
    {
        var results = await session.ListSeriesTypes(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }
}