namespace MetronWrapper.Test;

[TestFixture]
public class TestRole
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

    [Test(Description = "Test using the ListRoles function with a valid search")]
    public async Task TestListRoles()
    {
        var results = await _metron.ListRoles(parameters: new Dictionary<string, string> { { "name", "Writer" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo("Writer"));
        });
    }

    [Test(Description = "Test using the ListRoles function with an invalid search")]
    public async Task TestListRolesFail()
    {
        var results = await _metron.ListRoles(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }
}