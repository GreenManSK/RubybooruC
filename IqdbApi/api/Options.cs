using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IqdbApi.api
{
    class Options
    {
        public static readonly Options Default = new Options(new HashSet<ServiceType>(ServiceType.Values), false);

        ICollection<ServiceType> services { get; }
        bool IgnoreColors { get; } = false;

        public Options()
        {
        }

        public Options(ICollection<ServiceType> services, bool ignoreColors)
        {
            this.services = services;
            IgnoreColors = ignoreColors;
        }
    }
}
