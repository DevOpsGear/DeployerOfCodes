using Deployer.Services.Config.Interfaces;
using Json.NETMF;
using System.Collections;

namespace Deployer.Services.Config
{
	public class JsonPersistence : IJsonPersistence
	{
		private readonly ISmallTextFileIo _textIo;

		public JsonPersistence(ISmallTextFileIo textIo)
		{
			_textIo = textIo;
		}

		public Hashtable Read(string filePath)
		{
			var content = _textIo.Read(filePath);
			var hash = JsonSerializer.DeserializeString(content) as Hashtable;
			return hash ?? new Hashtable();
		}

		public void Write(string filePath, Hashtable hash)
		{
			var content = JsonSerializer.SerializeObject(hash);
			_textIo.Write(filePath, content);
		}
	}
}