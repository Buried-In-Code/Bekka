namespace MetronWrapper.Test.Schema;

[TestFixture]
public class ArcTest
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

    [Test(Description = "Test ListArcs with a valid search")]
    public async Task TestListArcs()
    {
        var results = await session.ListArcs(parameters: new Dictionary<string, string> { { "name", "Bone" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1491));
            Assert.That(results[0].Name, Is.EqualTo("\"Bone\" The Great Cow Race"));
        });
    }

    [Test(Description = "Test ListArcs with an invalid search")]
    public async Task TestListArcsFail()
    {
        var results = await session.ListArcs(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetArc with a valid id")]
    public async Task TestGetArc()
    {
        var result = await session.GetArc(id: 1491);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ComicvineId, Is.EqualTo(41751));
            Assert.That(result.Id, Is.EqualTo(1491));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/arc/2024/03/07/d75aba2ca26349c89c3104690d32cc2f.jpg"));
            Assert.That(result.Name, Is.EqualTo("\"Bone\" The Great Cow Race"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/arc/bone-the-great-cow-race/"));
        });
    }

    [Test(Description = "Test GetArc with an invalid id")]
    public void TestGetArcFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetArc(id: -1));
    }
}