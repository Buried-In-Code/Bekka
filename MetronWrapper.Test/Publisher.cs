namespace MetronWrapper.Test;

[TestFixture]
public class TestPublisher
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

    [Test(Description = "Test using the GetPublisher function with a valid Id")]
    public async Task TestGetPublisher()
    {
        var result = await _metron.GetPublisher(id: 19);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(19));
            Assert.That(result.Name, Is.EqualTo("Cartoon Books"));
            Assert.That(result.ComicvineId, Is.EqualTo(490));
            Assert.That(result.Founded, Is.EqualTo(1991));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/publisher/2019/01/21/cartoon-books.jpg"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/publisher/cartoon-books/"));
        });
    }

    [Test(Description = "Test using the GetPublisher function with an invalid Id")]
    public void TestGetPublisherFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetPublisher(id: -1));
    }

    [Test(Description = "Test using the ListPublishers function with a valid search")]
    public async Task TestListPublishers()
    {
        var results = await _metron.ListPublishers(parameters: new Dictionary<string, string> { { "name", "Cartoon Books" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(19));
            Assert.That(results[0].Name, Is.EqualTo("Cartoon Books"));
        });
    }

    [Test(Description = "Test using the ListPublishers function with an invalid search")]
    public async Task TestListPublishersFail()
    {
        var results = await _metron.ListPublishers(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test using the GetPublisherByComicvine function with a valid Id")]
    public async Task TestGetPublisherByComicvine()
    {
        var result = await _metron.GetPublisherByComicvine(comicvineId: 490);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(19));
            Assert.That(result.Name, Is.EqualTo("Cartoon Books"));
            Assert.That(result.ComicvineId, Is.EqualTo(490));
            Assert.That(result.Founded, Is.EqualTo(1991));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/publisher/2019/01/21/cartoon-books.jpg"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/publisher/cartoon-books/"));
        });
    }

    [Test(Description = "Test using the GetPublisherByComicvine function with an invalid Id")]
    public void TestGetPublisherByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetPublisherByComicvine(comicvineId: -1));
    }
}