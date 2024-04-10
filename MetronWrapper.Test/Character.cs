namespace MetronWrapper.Test;

[TestFixture]
public class TestCharacter
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

    [Test(Description = "Test using the GetCharacter function with a valid Id")]
    public async Task TestGetCharacter()
    {
        var result = await _metron.GetCharacter(id: 1234);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(1234));
            Assert.That(result.Name, Is.EqualTo("Smiley Bone"));
            Assert.That(result.ComicvineId, Is.EqualTo(23092));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/character/2019/01/21/Smiley-Bone.jpg"));
            Assert.That(result.Alias, Is.Empty);
            Assert.That(result.Creators[0].Id, Is.EqualTo(573));
            Assert.That(result.Teams, Is.Empty);
            Assert.That(result.Universes, Is.Empty);
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/character/smiley-bone/"));
        });
    }

    [Test(Description = "Test using the GetCharacter function with an invalid Id")]
    public void TestGetCharacterFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetCharacter(id: -1));
    }

    [Test(Description = "Test using the ListCharacters function with a valid search")]
    public async Task TestListCharacters()
    {
        var results = await _metron.ListCharacters(parameters: new Dictionary<string, string> { { "name", "Smiley Bone" } });
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(results[0].Id, Is.EqualTo(1234));
            Assert.That(results[0].Name, Is.EqualTo("Smiley Bone"));
        });
    }

    [Test(Description = "Test using the ListCharacters function with an invalid search")]
    public async Task TestListCharactersFail()
    {
        var results = await _metron.ListCharacters(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results, Is.Empty);
    }

    [Test(Description = "Test using the GetCharacterByComicvine function with a valid Id")]
    public async Task TestGetCharacterByComicvine()
    {
        var result = await _metron.GetCharacterByComicvine(comicvineId: 23092);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(1234));
            Assert.That(result.Name, Is.EqualTo("Smiley Bone"));
            Assert.That(result.ComicvineId, Is.EqualTo(23092));
            Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/character/2019/01/21/Smiley-Bone.jpg"));
            Assert.That(result.Alias, Is.Empty);
            Assert.That(result.Creators[0].Id, Is.EqualTo(573));
            Assert.That(result.Teams, Is.Empty);
            Assert.That(result.Universes, Is.Empty);
            Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/character/smiley-bone/"));
        });
    }

    [Test(Description = "Test using the GetCharacterByComicvine function with an invalid Id")]
    public void TestGetCharacterByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetCharacterByComicvine(comicvineId: -1));
    }
}