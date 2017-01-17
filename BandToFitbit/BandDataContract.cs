using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class BandDataContract
    {
        public List<Summary> summaries { get; set; }
        public string nextPage { get; set; }
        public int itemCount { get; set; }
    }
    public class HeartRateSummary
    {
        public string period { get; set; }
        public int? averageHeartRate { get; set; }
        public int? peakHeartRate { get; set; }
        public int? lowestHeartRate { get; set; }
    }

    public class CaloriesBurnedSummary
    {
        public string period { get; set; }
        public int? totalCalories { get; set; }
    }

    public class DistanceSummary
    {
        public string period { get; set; }
        public int? totalDistance { get; set; }
        public int? totalDistanceOnFoot { get; set; }
    }

    public class Summary
    {
        public string userId { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string parentDay { get; set; }
        public bool isTransitDay { get; set; }
        public string period { get; set; }
        public string duration { get; set; }
        public int stepsTaken { get; set; }
        public int floorsClimbed { get; set; }
        public CaloriesBurnedSummary caloriesBurnedSummary { get; set; }
        public HeartRateSummary heartRateSummary { get; set; }
        public DistanceSummary distanceSummary { get; set; }
        public List<string> dataSourceTypes { get; set; }
    }
}