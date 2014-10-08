namespace Deployer.Services.Api.Interfaces
{
	public interface IApiSocket
	{
		int Send(byte[] buffer, int size);
		void Send200_OK(string mimeType, int contentLength);
		void Send200_OK(string mimeType);
		void Send404_NotFound();
		void Send405_MethodNotAllowed();
		void Send500_Failure(string message = "");
	}
}