using GitHubAPIApp.DTOs;
using GitHubAPIApp.Exceptions.Concrete;
using GitHubAPIApp.Services.Abstract;
using Newtonsoft.Json;

namespace GitHubAPIApp.Services.Concrete
{
    public class GitHubClient : IGitHubClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<GitHubClient> _logger;

        public GitHubClient(IHttpClientFactory httpClientFactory, ILogger<GitHubClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<UserDTO> GetUserInfo(string login)
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient("App");

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"https://api.github.com/users/{login}");

            _logger.Log(LogLevel.Information, $"StatusCode {httpResponseMessage.StatusCode}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new DataNotFoundException((int)httpResponseMessage.StatusCode);
            }

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            UserDTO data = JsonConvert.DeserializeObject<UserDTO>(content);

            return data;
        }

        public async Task<IEnumerable<RepositoryDTO>> GetUserRepositories(string login)
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient("App");

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"https://api.github.com/users/{login}/repos");

            _logger.Log(LogLevel.Information, $"StatusCode {httpResponseMessage.StatusCode}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new DataNotFoundException((int)httpResponseMessage.StatusCode);
            }

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            IEnumerable<RepositoryDTO> data = JsonConvert.DeserializeObject<IEnumerable<RepositoryDTO>>(content);

            return data;
        }

        public async Task<Dictionary<string, float>> GetRepositoryLanguages(string login, string repositoryName)
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient("App");

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"https://api.github.com/repos/{login}/{repositoryName}/languages");

            _logger.Log(LogLevel.Information, $"StatusCode {httpResponseMessage.StatusCode}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new DataNotFoundException((int)httpResponseMessage.StatusCode);
            }

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            
            Dictionary<string, ulong> languagesBytes = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(content);

            var data = CalculatePercentageOfLanguageUse(languagesBytes);

            return data;

        }

        public async Task<Dictionary<string, float>> GetLanguagesFromAllUserRepositories(string login)
        {
            Dictionary<string, ulong> languagesBytesTotal = new();

            var repositories = GetUserRepositories(login);

            foreach (var repo in repositories.Result)
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient("App");

                HttpResponseMessage httpResponseMssg = await httpClient.GetAsync($"https://api.github.com/repos/{login}/{repo.Name}/languages");

                _logger.Log(LogLevel.Information, $"StatusCode {httpResponseMssg.StatusCode}");

                if (!httpResponseMssg.IsSuccessStatusCode)
                {
                    throw new DataNotFoundException((int)httpResponseMssg.StatusCode);
                }

                string cntnt = await httpResponseMssg.Content.ReadAsStringAsync();

                Dictionary<string, ulong> languagesBytesRepo = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(cntnt);

                foreach (var lang in languagesBytesRepo)
                {
                    if (!languagesBytesTotal.ContainsKey(lang.Key))
                    {
                        languagesBytesTotal.Add(lang.Key, lang.Value);
                    }
                    else
                    {
                        languagesBytesTotal[lang.Key] += lang.Value;
                    }
                }
            }

            Dictionary<string, float> languagePercentage = CalculatePercentageOfLanguageUse(languagesBytesTotal);

            return languagePercentage;
        }

        private Dictionary<string, float> CalculatePercentageOfLanguageUse(Dictionary<string, ulong> languagesBytes)
        {
            Dictionary<string, float> languagePercentage = new();

            ulong sum = 0;
            foreach (var lang in languagesBytes)
            {
                sum += lang.Value;
            }

            if (sum == 0)
            {
                return languagePercentage;
            }

            foreach (var lang in languagesBytes)
            {
                languagePercentage.Add(lang.Key, (float)lang.Value / sum * 100);
            }

            return languagePercentage;
        }
    }
}
