using GitHubAPIApp.DTOs;

namespace GitHubAPIApp.Services.Abstract
{
    public interface IGitHubClient
    {
        Task<UserDTO> GetUserInfo(string login);
        Task<IEnumerable<RepositoryDTO>> GetUserRepositories(string login);
        Task<Dictionary<string, float>> GetRepositoryLanguages(string login, string repositoryName);
        Task<Dictionary<string, float>> GetLanguagesFromAllUserRepositories(string login);
    }
}
