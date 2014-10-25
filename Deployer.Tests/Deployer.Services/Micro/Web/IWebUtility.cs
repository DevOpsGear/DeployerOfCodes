using System.Collections;

namespace Deployer.Services.Micro.Web
{
    public interface IWebUtility
    {
        void WriteJsonObject(IWebRequest req, object obj);
        Hashtable ReadJsonObject(IWebRequest req, int bufferSize);
        string ReadText(IWebRequest req, int bufferSize);
        string GetHttpBasicAuthToken(string username, string password);
        string NormalizeUrl(string url);
    }
}