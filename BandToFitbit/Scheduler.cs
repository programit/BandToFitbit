using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class Scheduler
    {
        private readonly TimeSpan durationBetweenRuns = TimeSpan.FromHours(6);
        private readonly BandDataProvider bandDataProvider = new BandDataProvider();
        private readonly FitbitDataProvider fitbitDataProvider = new FitbitDataProvider();

        public async Task Start()
        {
            while (true)
            {
                await this.Run();
                await Task.Delay(this.durationBetweenRuns);
            }
        }

        private async Task Run()
        {
            await this.BackFillAndGetNewData();
        }

        private async Task BackFillAndGetNewData()
        {
            FitbitGetStepsDataContract fitbitSteps = await this.fitbitDataProvider.GetSteps();
            BandDataContract bandSteps = await this.bandDataProvider.GetSteps();

            int bandTotalSteps = bandSteps.summaries.Sum(v => v.stepsTaken);
            int fitbitTotalSteps = fitbitSteps.steps.Sum(v => int.Parse(v.value));

            //TODO: Delete fitbit activity logs if the step count > 0

            await this.fitbitDataProvider.PublishSteps(bandTotalSteps);
        }
    }
}
