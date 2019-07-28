using System.Collections.Generic;

namespace IqdbApi.api
{
    public class Options
    {
        public static readonly Options Default = new Options(new HashSet<ServiceType>(ServiceType.Values), false);

        public ICollection<ServiceType> Services { get; }
        public bool IgnoreColors { get; }

        public Options()
        {
        }

        public Options(ICollection<ServiceType> services, bool ignoreColors)
        {
            Services = services;
            IgnoreColors = ignoreColors;
        }
    }
}