namespace MetronWrapper.Test;

[TestFixture]
public class ExceptionTest
{
    [Test(Description = "Test throwing an AuthenticationException")]
    public async Task TestAuthentication()
    {
        var session = new Metron(username: "Invalid", password: "Invalid", cache: null);
        Assert.ThrowsAsync<AuthenticationException>(async () => await session.GetIssue(id: 1088));
    }
 
    [Test(Description = "Test throwing a ServiceException for a 404")]
    public async Task Test404()
    {
        var username = Environment.GetEnvironmentVariable("METRON__USERNAME") ?? "IGNORED";
        var password = Environment.GetEnvironmentVariable("METRON__PASSWORD") ?? "IGNORED";
        var session = new Metron(username: username, password: password, cache: null);
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetIssue(id: -1));
    }
    
    [Test(Description = "Test throwing a ServiceException for a timeout")]
    public async Task TestTimeout()
    {
        var username = Environment.GetEnvironmentVariable("METRON__USERNAME") ?? "IGNORED";
        var password = Environment.GetEnvironmentVariable("METRON__PASSWORD") ?? "IGNORED";
        var session = new Metron(username: username, password: password, cache: null, timeout = TimeSpan.FromMillis(1));
        Assert.ThrowsAsync<ServiceException>(async () => await session.GetIssue(id: 1088));
    }
}