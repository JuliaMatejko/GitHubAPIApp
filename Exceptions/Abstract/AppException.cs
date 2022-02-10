namespace GitHubAPIApp.Exceptions.Abstract
{
    public abstract class AppException : Exception
    {
        public abstract string Code { get; }

        public AppException(string message) : base(message)
        {

        }
    }
}
