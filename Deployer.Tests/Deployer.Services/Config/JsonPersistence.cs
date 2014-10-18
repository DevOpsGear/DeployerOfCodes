using Deployer.Services.Config.Interfaces;
using Json.NETMF;
using System.Collections;

namespace Deployer.Services.Config
{
	public class JsonPersistence : IJsonPersistence
	{
		private readonly ISmallTextFileIo _io;

		public JsonPersistence(ISmallTextFileIo io)
		{
			_io = io;
		}

		public Hashtable Read(string filePath)
		{
			var content = _io.Read(filePath);
			var hash = JsonSerializer.DeserializeString(content) as Hashtable;
			return hash ?? new Hashtable();
		}

		public void Write(string filePath, Hashtable hash)
		{
			var content = JsonSerializer.SerializeObject(hash);
			_io.Write(filePath, content);
		}
	}
}