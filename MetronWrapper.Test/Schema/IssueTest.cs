namespace MetronWrapper.Test.Schema;

[TestFixture]
public class IssueTest
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

    [Test(Description = "Test ListIssues with a valid search")]
    public async Task TestListIssues()
    {
        var results = await session.ListIssues(parameters: new Dictionary<string, string> { { "series_id", 119.ToString() }, { "number", "1" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].CoverHash, Is.EqualTo("87386cc738ac7b38"));
            Assert.That(results[0].Image, Is.EqualTo("https://static.metron.cloud/media/issue/2019/01/21/bone-1.jpg"));
            Assert.That(results[0].StoreDate, Is.Null);
            Assert.That(results[0].CoverDate, Is.EqualTo(DateTime.Parse("1991-07-01")));
            Assert.That(results[0].Id, Is.EqualTo(1088));
            Assert.That(results[0].Number, Is.EqualTo("1"));
            Assert.That(results[0].Name, Is.EqualTo("Bone (1991) #1"));
            Assert.That(results[0].Series.Name, Is.EqualTo("Bone"));
            Assert.That(results[0].Series.Volume, Is.EqualTo(1));
            Assert.That(results[0].Series.YearBegan, Is.EqualTo(1991));
        });
    }

    [Test(Description = "Test ListIssues with an invalid search")]
    public async Task TestListIssuesFail()
    {
        var results = await session.ListIssues(parameters: new Dictionary<string, string> { { "series_id", 119.ToString() }, { "number", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetIssue with a valid id")]
    public async Task TestGetIssue()
    {
        var result = await session.GetIssue(id: 1088);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.CoverHash, Is.EqualTo("87386cc738ac7b38"));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/issue/2019/01/21/bone-1.jpg"));
            Assert.That(result.StoreDate, Is.Null);
            Assert.That(result.CoverDate, Is.EqualTo(DateTime.Parse("1991-07-01")));
            Assert.That(result.Id, Is.EqualTo(1088));
            Assert.That(result.Number, Is.EqualTo("1"));
            Assert.That(result.ComicvineId, Is.EqualTo(34352));
            Assert.That(result.Isbn, Is.Empty);
            Assert.That(result.PageCount, Is.EqualTo(28));
            Assert.That(result.Price, Is.EqualTo(2.95));
            Assert.That(result.Sku, Is.Empty);
            Assert.That(result.Title, Is.Empty);
            Assert.That(result.Upc, Is.Empty);
            Assert.That(result.Arcs, Is.Empty);
            Assert.That(result.Characters[0].Id, Is.EqualTo(1232));
            Assert.That(result.Credits[0].Id, Is.EqualTo(573));
            Assert.That(result.Reprints[0].Id, Is.EqualTo(113595));
            Assert.That(result.Stories[0], Is.EqualTo("The Map"));
            Assert.That(result.Teams[0].Id, Is.EqualTo(1473));
            Assert.That(result.Universes, Is.Empty);
            Assert.That(result.Variants, Is.Empty);
            Assert.That(result.Publisher.Id, Is.EqualTo(19));
            Assert.That(result.Publisher.Name, Is.EqualTo("Cartoon Books"));
            Assert.That(result.Rating.Id, Is.EqualTo(1));
            Assert.That(result.Rating.Name, Is.EqualTo("Unknown"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/issue/bone-1991-1/"));
            Assert.That(result.Series.Id, Is.EqualTo(119));
            Assert.That(result.Series.Name, Is.EqualTo("Bone"));
            Assert.That(result.Series.Genres, Is.Empty);
            Assert.That(result.Series.SeriesType.Id, Is.EqualTo(2));
            Assert.That(result.Series.SeriesType.Name, Is.EqualTo("Cancelled Series"));
            Assert.That(result.Series.SortName, Is.EqualTo("Bone"));
            Assert.That(result.Series.Volume, Is.EqualTo(1));
        });
    }

    [Test(Description = "Test GetIssue with an invalid id")]
    public void TestGetIssueFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetIssue(id: -1));
    }
}