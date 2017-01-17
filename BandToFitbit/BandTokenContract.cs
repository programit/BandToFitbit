using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class BandTokenContract
    {
        public int expires_in { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime requestDate { get; set; }
    }
}
