using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using log4net;

using MetronWrapper.Common;

namespace MetronWrapper;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var metron = new Metron(username: "", password: "");

        var publisherList = await metron.ListPublishers();
        Console.WriteLine($"Publishers: {publisherList.Count}");
        Console.WriteLine(publisherList[0].ToString());
        var comicvinePublisher = await metron.GetPublisherByComicvine(comicvine: 364);
        Console.WriteLine(comicvinePublisher);
        var publisher = await metron.GetPublisher(id: 3);
        Console.WriteLine(publisher);

        /*var seriesList = await metron.ListSeries(parameters: new Dictionary<string, string> { { "publisher_id", 3.ToString() } });
        Console.WriteLine($"Series: {seriesList.Count}");
        Console.WriteLine(seriesList[0].ToString());
        var comicvineSeries = await metron.GetSeriesByComicvine(comicvine: 104530);
        Console.WriteLine(comicvineSeries);
        var series = await metron.GetSeries(id: 2658);
        Console.WriteLine(series);

        var issueList = await metron.ListIssues(parameters: new Dictionary<string, string> { { "series_id", 2658.ToString() } });
        Console.WriteLine($"Issues: {issueList.Count}");
        Console.WriteLine(issueList[0].ToString());
        var comicvineIssue = await metron.GetIssueByComicvine(comicvine: 623161);
        Console.WriteLine(comicvineIssue);
        var issue = await metron.GetIssue(id: 37752);
        Console.WriteLine(issue);*/
    }
}

public class Metron
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(Metron));
    private static readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://metron.cloud/api/"),
        Timeout = TimeSpan.FromSeconds(30)
    };
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public Metron(string username, string password)
    {
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        _client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
        var runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Split(" ");
        _client.DefaultRequestHeaders.Add("User-Agent", $"MetronWrapper/0.1.0 ({Environment.OSVersion.Platform}/{Environment.OSVersion.Version}; {runtime[0]}/{runtime[1]})");

        _logger.Info("Initialized Metron");
    }

    private string QueryToString(Dictionary<string, string> query)
    {
        var builder = new StringBuilder("?");
        foreach (var entry in query)
            builder.Append($"{entry.Key}={entry.Value}&");
        return builder.ToString().TrimEnd('&');
    }

    private async Task<string?> PerformGetRequest(string endpoint, Dictionary<string, string>? parameters = null)
    {
        var query = parameters == null ? "" : QueryToString(query: parameters);
        try
        {
            var response = await _client.GetAsync($"{endpoint}/{query}");

            var statusCode = (int)response.StatusCode;
            if (statusCode >= 100 && statusCode < 200)
                _logger.Warn($"GET: {statusCode} - {endpoint}/{query}");
            else if (statusCode >= 200 && statusCode < 300)
                _logger.Debug($"GET: {statusCode} - {endpoint}/{query}");
            else if (statusCode >= 300 && statusCode < 400)
                _logger.Info($"GET: {statusCode} - {endpoint}/{query}");
            else if (statusCode >= 400 && statusCode < 500)
                _logger.Warn($"GET: {statusCode} - {endpoint}/{query}");
            else
                _logger.Error($"GET: {statusCode} - {endpoint}/query");

            if (statusCode == 200)
                return await response.Content.ReadAsStringAsync();
            _logger.Error(await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException hre)
        {
            _logger.Error($"Unable to make request to: {endpoint}/{query}", hre);
        }
        return null;
    }

    public async Task<List<BaseResource>> ListPublishers(Dictionary<string, string>? parameters = null)
    {
        try
        {
            var content = await PerformGetRequest(endpoint: "publisher", parameters: parameters);
            if (content == null)
                return new List<BaseResource>();
            var response = JsonSerializer.Deserialize<ListResponse<BaseResource>>(content, _options);
            if (response == null)
                return new List<BaseResource>();
            var results = response.Results;
            if (response.Next != null)
            {
                var _parameters = parameters ?? new Dictionary<string, string>();
                if (_parameters.ContainsKey("page"))
                    _parameters["page"] = (int.Parse(_parameters["page"]) + 1).ToString();
                else
                    _parameters["page"] = 2.ToString();
                results.AddRange(await ListPublishers(parameters: _parameters));
            }
            return results;
        }
        catch (JsonException je)
        {
            _logger.Error("Unable to parse response", je);
            return new List<BaseResource>();
        }
    }

    public async Task<Publisher?> GetPublisherByComicvine(long comicvine)
    {
        var results = await ListPublishers(parameters: new Dictionary<string, string> { { "cv_id", comicvine.ToString() } });
        var publisherId = results.FirstOrDefault()?.Id;
        if (publisherId == null)
            return null;
        return await GetPublisher(id: publisherId.Value);
    }

    public async Task<Publisher?> GetPublisher(long id)
    {
        try
        {
            var content = await PerformGetRequest(endpoint: $"publisher/{id}");
            if (content == null)
                return null;
            return JsonSerializer.Deserialize<Publisher>(content, _options);
        }
        catch (JsonException je)
        {
            _logger.Error("Unable to parse response", je);
            return null;

        }
    }

    public async Task<List<CommonSeries>> ListSeries(Dictionary<string, string>? parameters = null)
    {
        try
        {
            var content = await PerformGetRequest(endpoint: "series", parameters: parameters);
            if (content == null)
                return new List<CommonSeries>();
            var response = JsonSerializer.Deserialize<ListResponse<CommonSeries>>(content, _options);
            if (response == null)
                return new List<CommonSeries>();
            var results = response.Results;
            if (response.Next != null)
            {
                var _parameters = parameters ?? new Dictionary<string, string>();
                if (_parameters.ContainsKey("page"))
                    _parameters["page"] = (int.Parse(_parameters["page"]) + 1).ToString();
                else
                    _parameters["page"] = 2.ToString();
                results.AddRange(await ListSeries(parameters: _parameters));
            }
            return results;
        }
        catch (JsonException je)
        {
            _logger.Error("Unable to parse response", je);
            return new List<CommonSeries>();
        }
    }

    public async Task<Series?> GetSeriesByComicvine(long comicvine)
    {
        var results = await ListSeries(parameters: new Dictionary<string, string> { { "cv_id", comicvine.ToString() } });
        var seriesId = results.FirstOrDefault()?.Id;
        if (seriesId == null)
            return null;
        return await GetSeries(id: seriesId.Value);
    }

    public async Task<Series?> GetSeries(long id)
    {
        try
        {
            var content = await PerformGetRequest(endpoint: $"series/{id}");
            if (content == null)
                return null;
            return JsonSerializer.Deserialize<Series>(content, _options);
        }
        catch (JsonException je)
        {
            _logger.Error("Unable to parse response", je);
            return null;
        }
    }

    public async Task<List<CommonIssue>> ListIssues(Dictionary<string, string>? parameters = null)
    {
        try
        {
            var content = await PerformGetRequest(endpoint: "issue", parameters: parameters);
            if (content == null)
                return new List<CommonIssue>();
            var response = JsonSerializer.Deserialize<ListResponse<CommonIssue>>(content, _options);
            if (response == null)
                return new List<CommonIssue>();
            var results = response.Results;
            if (response.Next != null)
            {
                var _parameters = parameters ?? new Dictionary<string, string>();
                if (_parameters.ContainsKey("page"))
                    _parameters["page"] = (int.Parse(_parameters["page"]) + 1).ToString();
                else
                    _parameters["page"] = 2.ToString();
                results.AddRange(await ListIssues(parameters: _parameters));
            }
            return results;
        }
        catch (JsonException je)
        {
            _logger.Error("Unable to parse response", je);
            return new List<CommonIssue>();
        }
    }

    public async Task<Issue?> GetIssueByComicvine(long comicvine)
    {
        var results = await ListIssues(parameters: new Dictionary<string, string> { { "cv_id", comicvine.ToString() } });
        var issueId = results.FirstOrDefault()?.Id;
        if (issueId == null)
            return null;
        return await GetIssue(id: issueId.Value);
    }

    public async Task<Issue?> GetIssue(long id)
    {
        try
        {
            var content = await PerformGetRequest(endpoint: $"issue/{id}");
            if (content == null)
                return null;
            return JsonSerializer.Deserialize<Issue>(content, _options);
        }
        catch (JsonException je)
        {
            _logger.Error("Unable to parse response", je);
            return null;
        }
    }
}
