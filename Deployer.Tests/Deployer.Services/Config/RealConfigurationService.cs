using System;
using System.Collections;
using System.IO;
using Deployer.Services.Builders;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Models;

namespace Deployer.Services.Config
{
	public class RealConfigurationService : IConfigurationService
	{
		private readonly string _configDirectory;
		private readonly IJsonPersistence _persistence;
		private readonly ISlugCreator _slugCreator;

		public RealConfigurationService(string rootDirectory, IJsonPersistence persistence, ISlugCreator slugCreator)
		{
			_configDirectory = Path.Combine(rootDirectory, "config");
			_persistence = persistence;
			_slugCreator = slugCreator;
		}

		public ProjectModel[] GetProjects()
		{
			var projects = ReadConfigFile();
			return (ProjectModel[]) projects.ToArray(typeof(ProjectModel));
		}

		public ProjectModel GetProject(string slug)
		{
			var projects = ReadConfigFile();
			foreach(ProjectModel proj in projects)
			{
				if(proj.Slug == slug)
					return proj;
			}
			throw new ProjectDoesNotExistException(slug);
		}

		public void DeleteProject(string slug)
		{
			var projects = ReadConfigFile();
			RemoveBySlug(projects, slug);
			WriteConfigFile(projects);
		}

		public void SaveProject(ProjectModel newProject)
		{
			var projects = ReadConfigFile();
			if(newProject.Slug == "")
			{
				var existingSlugs = GetSlugs(projects);
				newProject.Slug = _slugCreator.CreateSlug(existingSlugs);
			}
			else
			{
				RemoveBySlug(projects, newProject.Slug);
			}
			projects.Add(newProject);
			WriteConfigFile(projects);
		}

		public Hashtable GetBuildParams(string slug)
		{
			var filePath = GetConfigFilePath(slug);
			return _persistence.Read(filePath);
		}

		public void SaveBuildParams(string slug, Hashtable config)
		{
			var filePath = GetConfigFilePath(slug);
			_persistence.Write(filePath, config);
		}

		#region Modify project array

		private static void RemoveBySlug(ArrayList projects, string slug)
		{
			var idxToReplace = -1;
			for(var idx = 0; idx < projects.Count; idx++)
			{
				var proj = (ProjectModel) projects[idx];
				if(proj.Slug == slug)
				{
					idxToReplace = idx;
					break;
				}
			}
			if(idxToReplace >= 0)
				projects.RemoveAt(idxToReplace);
			else
				throw new ProjectDoesNotExistException(slug);
		}

		private static string[] GetSlugs(ArrayList projects)
		{
			var slugs = new ArrayList();
			foreach(ProjectModel proj in projects)
			{
				slugs.Add(proj.Slug);
			}
			return (string[]) slugs.ToArray(typeof(string));
		}

		#endregion

		#region Read/write

		private ArrayList ReadConfigFile()
		{
			var list = _persistence.Read(ProjectListFilePath);
			var projects = new ArrayList();
			foreach(string slug in list.Keys)
			{
				var projectHash = list[slug] as Hashtable;
				if(projectHash == null) continue;
				var provider = Int32.Parse(projectHash["provider"].ToString());
				var prov = (BuildServiceProvider) provider;
				var proj = new ProjectModel(slug,
				                            projectHash["title"] as string,
				                            projectHash["subtitle"] as string,
				                            Int32.Parse(projectHash["rank"].ToString()),
				                            prov);
				projects.Add(proj);
			}
			return projects;
		}

		private void WriteConfigFile(IEnumerable projects)
		{
			var listHash = new Hashtable();

			foreach(ProjectModel proj in projects)
			{
				var projectHash = new Hashtable
					{
						{"title", proj.Title},
						{"subtitle", proj.Subtitle},
						{"rank", proj.Rank},
						{"provider", (int) proj.Provider}
					};
				listHash.Add(proj.Slug, projectHash);
			}

			_persistence.Write(ProjectListFilePath, listHash);
		}

		#endregion

		#region File paths

		private string ProjectListFilePath
		{
			get { return Path.Combine(_configDirectory, "projects.json"); }
		}

		private string GetConfigFilePath(string slug)
		{
			return Path.Combine(_configDirectory, slug + ".json");
		}

		#endregion
	}

	public class ProjectDoesNotExistException : Exception
	{
		public ProjectDoesNotExistException(string slug)
			: base(slug)
		{
		}
	}
}