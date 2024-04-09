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
        _metron = new Metron(username: username, password: password);
    }

    [Test(Description = "Test using the GetPublisher function with a valid Id")]
    public async Task TestGetPublisher()
    {
        var result = await _metron.GetPublisher(id: 3);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(364));
        Assert.That(result.Founded, Is.EqualTo(1986));
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/publisher/2018/11/16/dark-horse.jpg"));
        Assert.That(result.Name, Is.EqualTo("Dark Horse Comics"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/publisher/dark-horse-comics/"));
    }

    [Test(Description = "Test using the GetPublisher function with an invalid Id")]
    public void TestGetPublisherFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetPublisher(id: -1));
    }

    [Test(Description = "Test using the ListPublishers function with a valid search")]
    public async Task TestListPublishers()
    {
        var results = await _metron.ListPublishers(parameters: new Dictionary<string, string> { { "name", "Dark Horse Comics" } });
        Assert.That(results.Count, Is.EqualTo(1));

        Assert.That(results[0].Id, Is.EqualTo(3));
        Assert.That(results[0].Name, Is.EqualTo("Dark Horse Comics"));
    }

    [Test(Description = "Test using the ListPublishers function with an invalid search")]
    public async Task TestListPublishersFail()
    {
        var results = await _metron.ListPublishers(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test(Description = "Test using the GetPublisherByComicvine function with a valid Id")]
    public async Task TestGetPublisherByComicvine()
    {
        var result = await _metron.GetPublisherByComicvine(comicvineId: 364);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(364));
        Assert.That(result.Founded, Is.EqualTo(1986));
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/publisher/2018/11/16/dark-horse.jpg"));
        Assert.That(result.Name, Is.EqualTo("Dark Horse Comics"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/publisher/dark-horse-comics/"));
    }

    [Test(Description = "Test using the GetPublisherByComicvine function with an invalid Id")]
    public void TestGetPublisherByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetPublisherByComicvine(comicvineId: -1));
    }
}