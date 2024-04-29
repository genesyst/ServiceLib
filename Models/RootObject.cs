using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Models
{
    public class RootObject
    {
        public string place_id { get; set; }

        public string licence { get; set; }

        public string osm_type { get; set; }

        public string osm_id { get; set; }

        public string lat { get; set; }

        public string lon { get; set; }

        public string display_name { get; set; }

        public LocationAddress address { get; set; }
    }
}
