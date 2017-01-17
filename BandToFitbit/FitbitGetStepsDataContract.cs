using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class FitbitGetStepsDataContract
    {
        [JsonProperty("activities-steps")]
        public List<ActivitiesStep> steps { get; set; }
    }

    public class ActivitiesStep
    {
        public string dateTime { get; set; }
        public string value { get; set; }
    }
}