using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class FitbitPublishResponseContract
    {
        public ActivityLog activityLog { get; set; }
    }

    public class ActivityLog
    {
        public int activityId { get; set; }
        public int activityParentId { get; set; }
        public string activityParentName { get; set; }
        public int calories { get; set; }
        public string description { get; set; }
        public double distance { get; set; }
        public int duration { get; set; }
        public bool hasStartTime { get; set; }
        public bool isFavorite { get; set; }
        public string lastModified { get; set; }
        public long logId { get; set; }
        public string name { get; set; }
        public string startDate { get; set; }
        public string startTime { get; set; }
        public int steps { get; set; }
    }
}
