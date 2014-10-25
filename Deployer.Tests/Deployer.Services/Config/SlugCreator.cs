using System;
using Deployer.Services.Config.Interfaces;

// ReSharper disable StringIndexOfIsCultureSpecific.1
// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace Deployer.Services.Config
{
    public class SlugCreator : ISlugCreator
    {
        private const string Prefix = "project";

        public string CreateSlug(string[] existingSlugs)
        {
            var maxExisting = 0;
            foreach (var slug in existingSlugs)
            {
                var idxSep = slug.IndexOf("-");
                try
                {
                    var value = Int32.Parse(slug.Substring(idxSep + 1));
                    maxExisting = Math.Max(maxExisting, value);
                }
                catch
                {
                }
            }
            return Prefix + "-" + (maxExisting + 1).ToString();
        }
    }
}

// ReSharper restore StringIndexOfIsCultureSpecific.1
// ReSharper restore SpecifyACultureInStringConversionExplicitly