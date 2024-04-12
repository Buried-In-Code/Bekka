namespace MetronWrapper.Test;

[TestFixture]
public class TestTeam
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

    [Test(Description = "Test using the GetTeam function with a valid Id")]
    public async Task TestGetTeam()
    {
        var result = await _metron.GetTeam(id: 1473);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(1473));
            Assert.That(result.Name, Is.EqualTo("Rat Creatures"));
            Assert.That(result.ComicvineId, Is.EqualTo(62250));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/team/2024/03/07/f957fc534c0245abafbecb5e8bb4dafa.jpg"));
            Assert.That(result.Creators, Is.Empty);
            Assert.That(result.Universes, Is.Empty);
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/team/rat-creatures/"));
        });
    }

    [Test(Description = "Test using the GetTeam function with an invalid Id")]
    public void TestGetTeamFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetTeam(id: -1));
    }

    [Test(Description = "Test using the ListTeams function with a valid search")]
    public async Task TestListTeams()
    {
        var results = await _metron.ListTeams(parameters: new Dictionary<string, string> { { "name", "Rat Creatures" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1473));
            Assert.That(results[0].Name, Is.EqualTo("Rat Creatures"));
        });
    }

    [Test(Description = "Test using the ListTeams function with an invalid search")]
    public async Task TestListTeamsFail()
    {
        var results = await _metron.ListTeams(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test using the GetTeamByComicvine function with a valid Id")]
    public async Task TestGetTeamByComicvine()
    {
        var result = await _metron.GetTeamByComicvine(comicvineId: 62250);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(1473));
            Assert.That(result.Name, Is.EqualTo("Rat Creatures"));
            Assert.That(result.ComicvineId, Is.EqualTo(62250));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/team/2024/03/07/f957fc534c0245abafbecb5e8bb4dafa.jpg"));
            Assert.That(result.Creators, Is.Empty);
            Assert.That(result.Universes, Is.Empty);
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/team/rat-creatures/"));
        });
    }

    [Test(Description = "Test using the GetTeamByComicvine function with an invalid Id")]
    public void TestGetTeamByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetTeamByComicvine(comicvineId: -1));
    }
}