namespace MetronWrapper.Test;

[TestFixture]
public class TestArc
{
    private Metron _metron;

    [OneTimeSetUp]
    public void BeforeTests()
    {
        var username = Environment.GetEnvironmentVariable("METRON__USERNAME") ?? "IGNORED";
        var password = Environment.GetEnvironmentVariable("METRON__PASSWORD") ?? "IGNORED";
        _metron = new Metron(username: username, password: password);
    }

    [Test(Description = "Test using the GetArc function with a valid Id")]
    public async Task TestGetArc()
    {
        var result = await _metron.GetArc(id: 1491);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(41751));
        Assert.That(result.Id, Is.EqualTo(1491));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/arc/2024/03/07/d75aba2ca26349c89c3104690d32cc2f.jpg"));
        Assert.That(result.Name, Is.EqualTo("\"Bone\" The Great Cow Race"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/arc/bone-the-great-cow-race/"));
    }

    [Test(Description = "Test using the GetArc function with an invalid Id")]
    public void TestGetArcFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetArc(id: -1));
    }

    [Test(Description = "Test using the ListArcs function with a valid search")]
    public async Task TestListArcs()
    {
        var results = await _metron.ListArcs(parameters: new Dictionary<string, string> { { "name", "Bone" } });
        Assert.That(results.Count, Is.EqualTo(1));

        Assert.That(results[0].Id, Is.EqualTo(1491));
        Assert.That(results[0].Name, Is.EqualTo("\"Bone\" The Great Cow Race"));
    }

    [Test(Description = "Test using the ListArcs function with an invalid search")]
    public async Task TestListArcsFail()
    {
        var results = await _metron.ListArcs(parameters: new Dictionary<string, string> { { "name", "INVALID" } });
        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test(Description = "Test using the GetArcByComicvine function with a valid Id")]
    public async Task TestGetArcByComicvine()
    {
        var result = await _metron.GetArcByComicvine(comicvineId: 41751);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(41751));
        Assert.That(result.Id, Is.EqualTo(1491));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/arc/2024/03/07/d75aba2ca26349c89c3104690d32cc2f.jpg"));
        Assert.That(result.Name, Is.EqualTo("\"Bone\" The Great Cow Race"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/arc/bone-the-great-cow-race/"));
    }

    [Test(Description = "Test using the GetArcByComicvine function with an invalid Id")]
    public void TestGetArcByComicvineFail()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _metron.GetArcByComicvine(comicvineId: -1));
    }
}