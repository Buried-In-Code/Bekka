namespace Bekka.Test.Schema;

[TestFixture]
public class SeriesTest
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

    [Test(Description = "Test ListSeries with a valid search")]
    public async Task TestListSeries()
    {
        var results = await session.ListSeries(parameters: new Dictionary<string, string> { { "name", "Bone" } });
        Assert.That(results, Has.Count.EqualTo(8));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(119));
            Assert.That(results[0].IssueCount, Is.EqualTo(56));
            Assert.That(results[0].Name, Is.EqualTo("Bone (1991)"));
            Assert.That(results[0].Volume, Is.EqualTo(1));
            Assert.That(results[0].YearBegan, Is.EqualTo(1991));
        });
    }

    [Test(Description = "Test ListSeries with an invalid search")]
    public async Task TestListSeriesFail()
    {
        var results = await session.ListSeries(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetSeries with a valid id")]
    public async Task TestGetSeries()
    {
        var result = await session.GetSeries(id: 119);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Associated, Is.Empty);
            Assert.That(result.ComicvineId, Is.EqualTo(4691));
            Assert.That(result.Genres, Is.Empty);
            Assert.That(result.Id, Is.EqualTo(119));
            Assert.That(result.IssueCount, Is.EqualTo(56));
            Assert.That(result.Name, Is.EqualTo("Bone"));
            Assert.That(result.Publisher.Id, Is.EqualTo(19));
            Assert.That(result.Publisher.Name, Is.EqualTo("Cartoon Books"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/series/bone-1991/"));
            Assert.That(result.SeriesType.Id, Is.EqualTo(2));
            Assert.That(result.SeriesType.Name, Is.EqualTo("Cancelled Series"));
            Assert.That(result.SortName, Is.EqualTo("Bone"));
            Assert.That(result.Volume, Is.EqualTo(1));
            Assert.That(result.YearBegan, Is.EqualTo(1991));
            Assert.That(result.YearEnd, Is.EqualTo(1995));
        });
    }

    [Test(Description = "Test GetSeries with an invalid id")]
    public void TestGetSeriesFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetSeries(id: -1));
    }
}