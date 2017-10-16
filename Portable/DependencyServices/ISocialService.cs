using System.Threading.Tasks;

namespace dona.Forms.DependencyServices
{
    public interface ISocialService
    {
        Task PostAsync(string message);
        bool IsAuthenticated();
        string GetUsername();
    }

    public interface ITwitterService : ISocialService { }
    public interface IFacebookService : ISocialService { }
}
