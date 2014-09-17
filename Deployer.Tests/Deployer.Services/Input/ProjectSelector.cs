using System;
using Deployer.Services.Config;
using Deployer.Services.Hardware;
using Deployer.Services.Models;

namespace Deployer.Services.Input
{
	public class ProjectSelector : IProjectSelector
	{
		private readonly ICharDisplay _display;
		private readonly IConfigurationService _configService;
		private int _position;

		public ProjectSelector(ICharDisplay display, IConfigurationService configService)
		{
			_display = display;
			_configService = configService;
			Reset();
		}

		public void Reset()
		{
			_position = -1;
		}

		public void Up()
		{
			if (_position < 1)
				return;
			_position--;
			Refresh();
		}

		public void Down()
		{
			var projects = _configService.GetProjects();
			if (_position > (projects.Length - 2))
				return;
			_position++;
			Refresh();
		}

		public bool IsProjectSelected
		{
			get { return _position >= 0; }
		}

		public Project SelectedProject
		{
			get
			{
				if (IsProjectSelected)
				{
					var projects = _configService.GetProjects();
					var proj = projects[_position];
					return proj;
				}
				throw new Exception("No project selected");
			}
		}

		public string SelectedProjectName
		{
			get
			{
				if (IsProjectSelected)
				{
					var projects = _configService.GetProjects();
					var proj = projects[_position];
					return proj.Title;
				}
				throw new Exception("No project selected");
			}
		}

		private void Refresh()
		{
			var projects = _configService.GetProjects();
			var proj = projects[_position];
			_display.Write(proj.Title, proj.Subtitle);
		}
	}
}