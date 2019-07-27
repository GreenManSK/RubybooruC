using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    public class Options
    {
        public static readonly Options Default = new Options(new HashSet<ServiceType>(ServiceType.Values), false);

        public ICollection<ServiceType> Services { get; }
        public bool IgnoreColors { get; } = false;

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
