using System;

namespace Deployer.Services.Config
{
    public class ProjectDoesNotExistException : Exception
    {
        public ProjectDoesNotExistException(string slug)
            : base(slug)
        {
        }
    }
}