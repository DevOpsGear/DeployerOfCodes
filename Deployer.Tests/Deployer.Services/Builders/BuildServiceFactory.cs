using System;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;

namespace Deployer.Services.Builders
{
	public class BuildServiceFactory
	{
		public static IBuildService Create(BuildServiceProvider which,
		                                   IWebRequestFactory webFactory,
		                                   IWebUtility netio,
		                                   IGarbage garbage)
		{
			switch (which)
			{
				case BuildServiceProvider.AppVeyor:
					return new AppVeyorBuildService(webFactory, netio);

				case BuildServiceProvider.TeamCity:
					return new TeamCityBuildService(webFactory, netio);

				case BuildServiceProvider.Failing:
					return new FailingBuildService();

				case BuildServiceProvider.Succeeding:
					return new SuceedingBuildService();
			}
			throw new Exception("Invalid build service");
		}
	}
}