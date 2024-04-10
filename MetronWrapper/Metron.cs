using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using log4net;

using MetronWrapper.Schema;

namespace MetronWrapper
{
    public class Metron
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Metron));
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        private readonly HttpClient _client = new()
        {
            BaseAddress = new Uri("https://metron.cloud/api/"),
            Timeout = TimeSpan.FromSeconds(30)
        };
        private readonly SQLiteCache? _cache;

        public Metron(string username, string password, SQLiteCache? cache = null)
        {
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
            var runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Split(" ");
            _client.DefaultRequestHeaders.Add("User-Agent", $"MetronWrapper/0.1.0 ({Environment.OSVersion.Platform}/{Environment.OSVersion.Version}; {runtime[0]}/{runtime[1]})");

            _cache = cache;

            _logger.Info("Initialized Metron");
        }

        private static string QueryToString(Dictionary<string, string> query)
        {
            var builder = new StringBuilder("?");
            foreach (KeyValuePair<string, string> entry in query)
                builder.Append($"{entry.Key}={entry.Value}&");
            return builder.ToString().TrimEnd('&');
        }

        private async Task<string> PerformGetRequest(string endpoint, Dictionary<string, string>? parameters = null)
        {
            var query = parameters == null ? "" : QueryToString(query: parameters);
            try
            {
                var response = await _client.GetAsync($"{endpoint}/{query}");

                var statusCode = (int)response.StatusCode;
                if (statusCode is >= 100 and < 200)
                    _logger.Warn($"GET: {statusCode} - {endpoint}/{query}");
                else if (statusCode is >= 200 and < 300)
                    _logger.Debug($"GET: {statusCode} - {endpoint}/{query}");
                else if (statusCode is >= 300 and < 400)
                    _logger.Info($"GET: {statusCode} - {endpoint}/{query}");
                else if (statusCode is >= 400 and < 500)
                    _logger.Warn($"GET: {statusCode} - {endpoint}/{query}");
                else
                    _logger.Error($"GET: {statusCode} - {endpoint}/query");

                var content = await response.Content.ReadAsStringAsync();
                if (statusCode == 200)
                    return content;
                if (statusCode == 404)
                    throw new ServiceException("Resource not found");

                var mappedContent = JsonSerializer.Deserialize<dynamic>(content, _options) ?? throw new ServiceException($"Unable to parse response from `{endpoint}/{query}` as Json");
                _logger.Error(mappedContent);
                if (statusCode == 401)
                    throw new AuthenticationException(mappedContent.GetProperty("detail").GetString());
                throw new ServiceException(mappedContent.GetProperty("detail").GetString());
            }
            catch (JsonException je)
            {
                throw new ServiceException($"Unable to parse response from `{endpoint}/{query}` as Json", je);
            }
        }

        public async Task<string> GetRequest(string endpoint, Dictionary<string, string>? parameters = null)
        {
            var query = parameters == null ? "" : QueryToString(query: parameters);
            if (_cache != null)
            {
                var cachedResponse = _cache.Select(url: $"{endpoint}/{query}");
                if (cachedResponse != null)
                {
                    _logger.Debug($"Using cached response for: {endpoint}/{query}");
                    return cachedResponse;
                }
            }
            var response = await PerformGetRequest(endpoint: endpoint, parameters: parameters);
            _cache?.Insert(url: $"{endpoint}/{query}", response: response);
            return response;
        }

        public async Task<List<BaseResource>> ListArcs(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "arc", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<BaseResource>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListArcs(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<Arc> GetArcByComicvine(long comicvineId)
        {
            var results = await ListArcs(parameters: new Dictionary<string, string> { { "cv_id", comicvineId.ToString() } });
            var arcId = results.FirstOrDefault()?.Id ?? throw new ServiceException("Resource not found");
            return await GetArc(id: arcId);
        }

        public async Task<Arc> GetArc(long id)
        {
            try
            {
                var content = await GetRequest(endpoint: $"arc/{id}");
                return JsonSerializer.Deserialize<Arc>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);

            }
        }

        public async Task<List<BaseResource>> ListCharacters(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "character", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<BaseResource>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListCharacters(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<Character> GetCharacterByComicvine(long comicvineId)
        {
            var results = await ListCharacters(parameters: new Dictionary<string, string> { { "cv_id", comicvineId.ToString() } });
            var characterId = results.FirstOrDefault()?.Id ?? throw new ServiceException("Resource not found");
            return await GetCharacter(id: characterId);
        }

        public async Task<Character> GetCharacter(long id)
        {
            try
            {
                var content = await GetRequest(endpoint: $"character/{id}");
                return JsonSerializer.Deserialize<Character>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);

            }
        }

        public async Task<List<BaseResource>> ListCreators(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "creator", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<BaseResource>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListCreators(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<Creator> GetCreatorByComicvine(long comicvineId)
        {
            var results = await ListCreators(parameters: new Dictionary<string, string> { { "cv_id", comicvineId.ToString() } });
            var creatorId = results.FirstOrDefault()?.Id ?? throw new ServiceException("Resource not found");
            return await GetCreator(id: creatorId);
        }

        public async Task<Creator> GetCreator(long id)
        {
            try
            {
                var content = await GetRequest(endpoint: $"creator/{id}");
                return JsonSerializer.Deserialize<Creator>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);

            }
        }

        public async Task<List<CommonIssue>> ListIssues(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "issue", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<CommonIssue>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListIssues(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<Issue> GetIssueByComicvine(long comicvineId)
        {
            var results = await ListIssues(parameters: new Dictionary<string, string> { { "cv_id", comicvineId.ToString() } });
            var issueId = results.FirstOrDefault()?.Id ?? throw new ServiceException("Resource not found");
            return await GetIssue(id: issueId);
        }

        public async Task<Issue> GetIssue(long id)
        {
            try
            {
                var content = await GetRequest(endpoint: $"issue/{id}");
                return JsonSerializer.Deserialize<Issue>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<List<BaseResource>> ListPublishers(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "publisher", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<BaseResource>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListPublishers(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<Publisher> GetPublisherByComicvine(long comicvineId)
        {
            var results = await ListPublishers(parameters: new Dictionary<string, string> { { "cv_id", comicvineId.ToString() } });
            var publisherId = results.FirstOrDefault()?.Id ?? throw new ServiceException("Resource not found");
            return await GetPublisher(id: publisherId);
        }

        public async Task<Publisher> GetPublisher(long id)
        {
            try
            {
                var content = await GetRequest(endpoint: $"publisher/{id}");
                return JsonSerializer.Deserialize<Publisher>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);

            }
        }

        public async Task<List<GenericItem>> ListRoles(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "role", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<GenericItem>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListRoles(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<List<CommonSeries>> ListSeries(Dictionary<string, string>? parameters = null)
        {
            try
            {
                var content = await GetRequest(endpoint: "series", parameters: parameters);
                var response = JsonSerializer.Deserialize<ListResponse<CommonSeries>>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
                var results = response.Results;
                if (response.Next != null)
                {
                    var _parameters = parameters ?? [];
                    _parameters["page"] = _parameters.TryGetValue("page", out string? value) ? (int.Parse(value) + 1).ToString() : 2.ToString();
                    results.AddRange(await ListSeries(parameters: _parameters));
                }
                return results;
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }

        public async Task<Series> GetSeriesByComicvine(long comicvineId)
        {
            var results = await ListSeries(parameters: new Dictionary<string, string> { { "cv_id", comicvineId.ToString() } });
            var seriesId = results.FirstOrDefault()?.Id ?? throw new ServiceException("Resource not found");
            return await GetSeries(id: seriesId);
        }

        public async Task<Series> GetSeries(long id)
        {
            try
            {
                var content = await GetRequest(endpoint: $"series/{id}");
                return JsonSerializer.Deserialize<Series>(content, _options) ?? throw new ServiceException("Unable to parse response as Json");
            }
            catch (JsonException je)
            {
                throw new ServiceException("Unable to parse response as Json", je);
            }
        }
    }
}