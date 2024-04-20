namespace MetronWrapper.Test.Schema;

[TestFixture]
public class TeamTest
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

    [Test(Description = "Test ListTeams with a valid search")]
    public async Task TestListTeams()
    {
        var results = await session.ListTeams(parameters: new Dictionary<string, string> { { "name", "Rat Creatures" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1473));
            Assert.That(results[0].Name, Is.EqualTo("Rat Creatures"));
        });
    }

    [Test(Description = "Test ListTeams with an invalid search")]
    public async Task TestListTeamsFail()
    {
        var results = await session.ListTeams(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetTeam with a valid id")]
    public async Task TestGetTeam()
    {
        var result = await session.GetTeam(id: 1473);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.ComicvineId, Is.EqualTo(62250));
            Assert.That(result.Creators, Is.Empty);
            Assert.That(result.Id, Is.EqualTo(1473));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/team/2024/03/07/f957fc534c0245abafbecb5e8bb4dafa.jpg"));
            Assert.That(result.Name, Is.EqualTo("Rat Creatures"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/team/rat-creatures/"));
            Assert.That(result.Universes, Is.Empty);
        });
    }

    [Test(Description = "Test GetTeam with an invalid id")]
    public void TestGetTeamFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetTeam(id: -1));
    }
}