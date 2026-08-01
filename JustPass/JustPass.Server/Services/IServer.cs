using JustPass.Server.Models;

namespace JustPass.Server.Services
{
    public interface IServer
    {
        PassResponse GeneratedPWD(PassRequest request);
        bool ValidationVls(string pwd);
        int getSafety(string pwd);
        List<PassHistory> GetHistory();
        void ClearHistory();    
    }
}
