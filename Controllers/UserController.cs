using GitHubAPIApp.DTOs;
using GitHubAPIApp.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace GitHubAPIApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGitHubClient _gitHubClient;

        public UserController(ILogger<UserController> logger, IGitHubClient gitHubClient)
        {
            _logger = logger;
            _gitHubClient = gitHubClient;
        }

        /// <summary>
        /// Test the availability of the service
        /// Short description of this application
        /// </summary>
        /// <returns></returns>
        [HttpGet("about")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public string About()
        {
            const string response = "This app gets information about user and user's repositories (repository name and used languages) from GitHub API and calculates percentage of programming language usage among all repositories.";
            return response;
        }

        /// <summary>
        /// Get single user data
        /// </summary>
        /// <br/>
        /// Used to obtain GitHub user data.
        /// <br/>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet("{login}")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<UserDTO>> GetUserInfo(string login)
        {
            UserDTO dto = await _gitHubClient.GetUserInfo(login);

            if (dto is null) return NotFound();

            return dto;
        }

        /// <summary>
        /// Get user repositories.
        /// </summary>
        /// <br/>
        /// Used to obtain GitHub user repositories.
        /// <br/>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet("{login}/repositories")]
        [ProducesResponseType(typeof(IEnumerable<RepositoryDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IEnumerable<RepositoryDTO>> GetUserRepositories(string login)
        {
            IEnumerable<RepositoryDTO> dto = await _gitHubClient.GetUserRepositories(login);

            if (dto is null) return (IEnumerable<RepositoryDTO>)NotFound();

            return dto;
        }

        /// <summary>
        /// Get repository languages.
        /// </summary>
        /// <br/>
        /// Used to obtain programming languages used in user repository.
        /// <br/>
        /// <param name="login"></param>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        [HttpGet("{login}/{repositoryName}/languages")]
        [ProducesResponseType(typeof(Dictionary<string, float>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<Dictionary<string, float>>> GetRepositoryLanguages(string login, string repositoryName)
        {
            Dictionary<string, float> dto = await _gitHubClient.GetRepositoryLanguages(login, repositoryName);

            if (dto is null) return NotFound();

            return dto;
        }

        /// <summary>
        /// Get languages from all repositories.
        /// </summary>
        /// <br/>
        /// Used to obtain programming languages used in all user repositories.
        /// <br/>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet("{login}/languages")]
        [ProducesResponseType(typeof(Dictionary<string, float>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<Dictionary<string, float>>> GetLanguagesFromAllUserRepositories(string login)
        {
            Dictionary<string, float> dto = await _gitHubClient.GetLanguagesFromAllUserRepositories(login);

            if (dto is null) return NotFound();

            return dto;
        }
    }
}
