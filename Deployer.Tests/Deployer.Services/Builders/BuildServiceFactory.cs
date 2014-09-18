using System;
using Deployer.Services.Micro;

namespace Deployer.Services.Builders
{
	public class BuildServiceFactory
	{
		public static IBuildService Create(BuildServiceProvider which, IWebRequestFactory webFactory, IGarbage garbage)
		{
			switch (which)
			{
				case BuildServiceProvider.AppVeyor:
					return new AppVeyorBuildService(webFactory, garbage);

				case BuildServiceProvider.TeamCity:
					return new TeamCityBuildService(webFactory, garbage);

				case BuildServiceProvider.Failing:
					return new FailingBuildService();

				case BuildServiceProvider.Succeeding:
					return new SuceedingBuildService();
			}
			throw new Exception("Invalid build service");
		}
	}
}