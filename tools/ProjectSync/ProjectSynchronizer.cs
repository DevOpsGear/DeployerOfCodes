using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ProjectSync
{
	public class ProjectSynchronizer
	{
		private readonly XmlNamespaceManager _namespaceManager;
		private readonly XNamespace _ns;

		public ProjectSynchronizer()
		{
			_namespaceManager = new XmlNamespaceManager(new NameTable());
			_namespaceManager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

			_ns = @"http://schemas.microsoft.com/developer/msbuild/2003";
		}

		public void Sync(string pathSource, string pathTarget, string relative)
		{
			var source = XDocument.Load(pathSource);
			var target = XDocument.Load(pathTarget);

			var targetGroup = GutTarget(target);
			if (targetGroup == null)
				return;

			var sourceFiles = source.XPathSelectElements("//msbuild:ItemGroup/msbuild:Compile", _namespaceManager);
			foreach (var sourceFile in sourceFiles)
			{
				var attrInclude = sourceFile.Attribute(XName.Get("Include"));
				if (attrInclude == null) continue;
				if (attrInclude.Value.StartsWith(@"Properties\")) continue;

				var newIncludeText = relative + attrInclude.Value;
				var newLinkText = attrInclude.Value;

				var compileElement = new XElement(_ns + "Compile");
				compileElement.SetAttributeValue("Include", newIncludeText);
				var linkElement = new XElement(_ns + "Link");
				;
				linkElement.SetValue(newLinkText);

				compileElement.Add(linkElement);
				targetGroup.Add(compileElement);
			}

            target.Save(pathTarget);
		}

		private XElement GutTarget(XDocument target)
		{
			XElement targetGroup = null;
			var elementsToRemove = new List<XElement>();
			var targetsToRemove = target.XPathSelectElements("//msbuild:ItemGroup/msbuild:Compile", _namespaceManager);
			foreach (var targetToRemove in targetsToRemove)
			{
				targetGroup = targetToRemove.Parent;

				var attrInclude = targetToRemove.Attribute(XName.Get("Include"));
				if (attrInclude == null) continue;
				if (attrInclude.Value.StartsWith(@"..\Deployer.Tests"))
					elementsToRemove.Add(targetToRemove);
			}

			elementsToRemove.ForEach(x => x.Remove());

			return targetGroup;
		}
	}
}