namespace MetronWrapper.Test;

[TestFixture]
public class TestCreator
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

    [Test(Description = "Test using the GetCreator function with a valid Id")]
    public async Task TestGetCreator()
    {
        var result = await _metron.GetCreator(id: 573);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(573));
            Assert.That(result.Name, Is.EqualTo("Jeff Smith"));
            Assert.That(result.Birth, Is.EqualTo(DateTime.Parse("1960-02-27")));
            Assert.That(result.ComicvineId, Is.EqualTo(23088));
            Assert.That(result.Death, Is.Null);
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/creator/2018/12/06/jeff_smith.jpg"));
            Assert.That(result.Alias, Is.Empty);
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/creator/jeff-smith/"));
        });
    }

    [Test(Description = "Test using the GetCreator function with an invalid Id")]
    public void TestGetCreatorFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetCreator(id: -1));
    }

    [Test(Description = "Test using the ListCreators function with a valid search")]
    public async Task TestListCreators()
    {
        var results = await _metron.ListCreators(parameters: new Dictionary<string, string> { { "name", "Jeff Smith" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(573));
            Assert.That(results[0].Name, Is.EqualTo("Jeff Smith"));
        });
    }

    [Test(Description = "Test using the ListCreators function with an invalid search")]
    public async Task TestListCreatorsFail()
    {
        var results = await _metron.ListCreators(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test using the GetCreatorByComicvine function with a valid Id")]
    public async Task TestGetCreatorByComicvine()
    {
        var result = await _metron.GetCreatorByComicvine(comicvineId: 23088);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(573));
            Assert.That(result.Name, Is.EqualTo("Jeff Smith"));
            Assert.That(result.Birth, Is.EqualTo(DateTime.Parse("1960-02-27")));
            Assert.That(result.ComicvineId, Is.EqualTo(23088));
            Assert.That(result.Death, Is.Null);
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/creator/2018/12/06/jeff_smith.jpg"));
            Assert.That(result.Alias, Is.Empty);
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/creator/jeff-smith/"));
        });
    }

    [Test(Description = "Test using the GetCreatorByComicvine function with an invalid Id")]
    public void TestGetCreatorByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetCreatorByComicvine(comicvineId: -1));
    }
}