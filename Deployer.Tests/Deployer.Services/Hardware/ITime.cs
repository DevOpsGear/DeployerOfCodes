using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Services.Hardware
{
	public interface ITimeService
	{
		DateTime Now();
	}
}