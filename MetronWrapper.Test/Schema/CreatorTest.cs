namespace MetronWrapper.Test.Schema;

[TestFixture]
public class CreatorTest
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

    [Test(Description = "Test ListCreators with a valid search")]
    public async Task TestListCreators()
    {
        var results = await session.ListCreators(parameters: new Dictionary<string, string> { { "name", "Jeff Smith" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(573));
            Assert.That(results[0].Name, Is.EqualTo("Jeff Smith"));
        });
    }

    [Test(Description = "Test ListCreators with an invalid search")]
    public async Task TestListCreatorsFail()
    {
        var results = await session.ListCreators(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetCreator with a valid id")]
    public async Task TestGetCreator()
    {
        var result = await session.GetCreator(id: 573);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Alias, Is.Empty);
            Assert.That(result.Birth, Is.EqualTo(DateTime.Parse("1960-02-27")));
            Assert.That(result.ComicvineId, Is.EqualTo(23088));
            Assert.That(result.Death, Is.Null);
            Assert.That(result.Id, Is.EqualTo(573));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/creator/2018/12/06/jeff_smith.jpg"));
            Assert.That(result.Name, Is.EqualTo("Jeff Smith"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/creator/jeff-smith/"));
        });
    }

    [Test(Description = "Test GetCreator with an invalid id")]
    public void TestGetCreatorFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetCreator(id: -1));
    }
}