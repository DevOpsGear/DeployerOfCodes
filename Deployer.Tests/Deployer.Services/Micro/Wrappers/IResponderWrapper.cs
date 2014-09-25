using System.Collections;
using System.IO;

namespace Deployer.Services.Micro.Wrappers
{
	public interface IResponderWrapper
	{
		//     Parameters appended to the URL.
		IDictionary UrlParameters { get; }

		//     Wraps the content for PUT and POST requests.
		IBodyWrapper BodyWrapper { get; }

		//     The client's IP address.
		string ClientEndpoint { get; }

		//     The header content of the message.
		byte[] HeaderData { get; }

		//     Gets the Http method as specified by the Gadgeteer.Networking.Responder.HttpMethod
		//     enumeration.
		HttpMethod HttpMethod { get; }

		// Summary:
		//     The http version.
		string HttpVersion { get; }

		// Summary:
		//     The path of the addressed web event.
		string Path { get; }

		//     Whether or not this web responder has been responded to.
		bool Responded { get; }

		// Summary:
		//     Gets the value of the specified header field or null.
		//
		// Parameters:
		//   name:
		//     A string that identifies the name of the header field.
		//
		// Returns:
		//     The value of the specified header field.
		string GetHeaderField(string name);

		// Summary:
		//     Gets the value of the specified url parameter or null.
		//
		// Parameters:
		//   name:
		//     A string that identifies the name of the parameter.
		//
		// Returns:
		//     The value of the parameter.
		string GetParameterValueFromUrl(string name);

		// Summary:
		//     Updates the data with which the web event responds and sets the correct MIME
		//     type.
		//
		// Parameters:
		//   audioStream:
		//     An audio/mp3 stream to be published.
		void Respond(Stream audioStream);

		// Summary:
		//     Updates the data with which the web event responds and sets the correct MIME
		//     type.
		//
		// Parameters:
		//   text:
		//     The plain/text to be published.
		void Respond(string text);

		// Summary:
		//     Updates the data with which the WebEvent responds. The MIME type can be set
		//     manually.
		//
		// Parameters:
		//   data:
		//     The data to be streamed.
		//
		//   contentType:
		//     The MIME type of the outgoing data.
		void Respond(byte[] data, string contentType);
	}
}