using GitHubAPIApp.Exceptions.Abstract;

namespace GitHubAPIApp.Exceptions.Concrete
{
    public class DataNotFoundException : AppException
    {
        public DataNotFoundException(int statusCode) : base($"Get data operation resulted with Not Found exception, exited with code {statusCode}")
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }

        public override string Code => "get_file_result_exception";
    }
}
