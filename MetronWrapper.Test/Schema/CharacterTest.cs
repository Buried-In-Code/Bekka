namespace MetronWrapper.Test.Schema;

[TestFixture]
public class CharacterTest
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

    [Test(Description = "Test ListCharacters with a valid search")]
    public async Task TestListCharacters()
    {
        var results = await session.ListCharacters(parameters: new Dictionary<string, string> { { "name", "Smiley Bone" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1234));
            Assert.That(results[0].Name, Is.EqualTo("Smiley Bone"));
        });
    }

    [Test(Description = "Test ListCharacters with an invalid search")]
    public async Task TestListCharactersFail()
    {
        var results = await session.ListCharacters(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test GetCharacter with a valid id")]
    public async Task TestGetCharacter()
    {
        var result = await session.GetCharacter(id: 1234);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Alias, Is.Empty);
            Assert.That(result.ComicvineId, Is.EqualTo(23092));
            Assert.That(result.Creators[0].Id, Is.EqualTo(573));
            Assert.That(result.Id, Is.EqualTo(1234));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/character/2019/01/21/Smiley-Bone.jpg"));
            Assert.That(result.Name, Is.EqualTo("Smiley Bone"));
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/character/smiley-bone/"));
            Assert.That(result.Teams, Is.Empty);
            Assert.That(result.Universes, Is.Empty);
        });
    }

    [Test(Description = "Test GetCharacter with an invalid id")]
    public void TestGetCharacterFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetCharacter(id: -1));
    }
}