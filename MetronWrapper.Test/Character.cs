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
        _metron = new Metron(username: username, password: password);
    }

    [Test(Description = "Test using the GetCharacter function with a valid Id")]
    public async Task TestGetCharacter()
    {
        var result = await _metron.GetCharacter(id: 1234);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(23092));
        Assert.That(result.Creators[0].Id, Is.EqualTo(573));
        Assert.That(result.Id, Is.EqualTo(1234));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/character/2019/01/21/Smiley-Bone.jpg"));
        Assert.That(result.Name, Is.EqualTo("Smiley Bone"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/character/smiley-bone/"));
        Assert.That(result.Teams, Is.Empty);
        Assert.That(result.Universes, Is.Empty);
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
        Assert.That(results.Count, Is.EqualTo(1));

        Assert.That(results[0].Id, Is.EqualTo(1234));
        Assert.That(results[0].Name, Is.EqualTo("Smiley Bone"));
    }

    [Test(Description = "Test using the ListCharacters function with an invalid search")]
    public async Task TestListArcsFail()
    {
        var results = await _metron.ListCharacters(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test(Description = "Test using the GetCharacterByComicvine function with a valid Id")]
    public async Task TestGetCharacterByComicvine()
    {
        var result = await _metron.GetCharacterByComicvine(comicvineId: 23092);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(23092));
        Assert.That(result.Creators[0].Id, Is.EqualTo(573));
        Assert.That(result.Id, Is.EqualTo(1234));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/character/2019/01/21/Smiley-Bone.jpg"));
        Assert.That(result.Name, Is.EqualTo("Smiley Bone"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/character/smiley-bone/"));
        Assert.That(result.Teams, Is.Empty);
        Assert.That(result.Universes, Is.Empty);
    }

    [Test(Description = "Test using the GetCharacterByComicvine function with an invalid Id")]
    public void TestGetCharacterByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetCharacterByComicvine(comicvineId: -1));
    }
}