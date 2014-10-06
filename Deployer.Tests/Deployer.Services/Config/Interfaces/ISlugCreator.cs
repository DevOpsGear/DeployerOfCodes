namespace Deployer.Services.Config.Interfaces
{
	public interface ISlugCreator
	{
		string CreateSlug(string[] existingSlugs);
	}
}