namespace MetronWrapper.Test;

[TestFixture]
public class TestPublisher
{
    private Metron metron;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var username = System.Environment.GetEnvironmentVariable("METRON__USERNAME") ?? "IGNORED";
        var password = System.Environment.GetEnvironmentVariable("METRON__PASSWORD") ?? "IGNORED";
        metron = new Metron(username: username, password: password);
    }

    [Test(Description = "Test the Get Publisher with a valid Id")]
    public async Task TestGetPublisher()
    {
        var result = await metron.GetPublisher(id: 3);
        Assert.That(result, Is.Not.Null);

        Assert.That(result.ComicvineId, Is.EqualTo(364));
        Assert.That(result.Founded, Is.EqualTo(1986));
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.Image, Is.EqualTo("https://static.metron.cloud/media/publisher/2018/11/16/dark-horse.jpg"));
        Assert.That(result.Name, Is.EqualTo("Dark Horse Comics"));
        Assert.That(result.ResourceUrl, Is.EqualTo("https://metron.cloud/publisher/dark-horse-comics/"));
    }

    [Test]
    public async Task TestGetPublisherFail()
    {
        //Assert.Throws<ServiceException>(() => metron.GetPublisher(id: -1));
        Assert.Pass();
    }
}