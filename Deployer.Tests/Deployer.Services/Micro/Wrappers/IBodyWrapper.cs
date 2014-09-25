using System.IO;

namespace Deployer.Services.Micro.Wrappers
{
	public interface IBodyWrapper
	{
		//     Gets the content type of the incoming data.
		string ContentType { get; }

		//     Gets the posted data as a byte[] array.
		byte[] RawContent { get; }

		//     Gets the posted data as a stream, or returns null.
		Stream Stream { get; }

		//     Gets the incoming posted data as text, or returns null.
		string Text { get; }
	}
}