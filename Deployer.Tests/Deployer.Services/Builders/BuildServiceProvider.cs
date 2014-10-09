namespace Deployer.Services.Builders
{
	public enum BuildServiceProvider
	{
		None = 0,
		Failing = 1,
		Succeeding = 2,
		AppVeyor = 3,
		TeamCity = 4
	}
}