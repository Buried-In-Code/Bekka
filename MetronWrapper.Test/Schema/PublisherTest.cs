namespace MetronWrapper.Test.Schema;

[TestFixture]
public class PublisherTest
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

    [Test(Description = "Test ListPublishers with a valid search")]
    public async Task TestListPublishers()
    {
        var results = await session.ListPublishers(parameters: new Dictionary<string, string> { { "name", "Cartoon Books" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(19));
            Assert.That(results[0].Name, Is.EqualTo("Cartoon Books"));
        });
    }

    [Test(Description = "Test ListPublishers with an invalid search")]
    public async Task TestListPublishersFail()
    {
        var results = await session.ListPublishers(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetPublisher with a valid id")]
    public async Task TestGetPublisher()
    {
        var result = await session.GetPublisher(id: 19);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ComicvineId, Is.EqualTo(490));
            Assert.That(result.Founded, Is.EqualTo(1991));
            Assert.That(result.Id, Is.EqualTo(19));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/publisher/2019/01/21/cartoon-books.jpg"));
            Assert.That(result.Name, Is.EqualTo("Cartoon Books"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/publisher/cartoon-books/"));
        });
    }

    [Test(Description = "Test GetPublisher with an invalid id")]
    public void TestGetPublisherFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetPublisher(id: -1));
    }
}