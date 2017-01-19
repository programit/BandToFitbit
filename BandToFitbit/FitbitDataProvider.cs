using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class FitbitDataProvider
    {       
        private const string AuthTokenType = "Bearer";
        private const string AuthHeader = "Authorization";

        //{0} should be in form YYYY-MM-DD
        private const string GetStepsForDayUrl = "https://api.fitbit.com/1/user/-/activities/steps/date/{0}/1d.json";

        private const string PublishStepsUrl = "https://api.fitbit.com/1/user/-/activities.json?activityId=90013&startTime=00%3A00&durationMillis=86400000&date={0}&distance={1}&distanceUnit=steps";

        private const string DeleteStepsUrl = "https://api.fitbit.com/1/user/-/activities/{0}.json";

        public async Task PublishSteps(int steps)
        {
            HttpResponseMessage responseMessage = await this.TryPublish(steps);
            if(responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            string responseString = await responseMessage.Content.ReadAsStringAsync();
            FitbitPublishResponseContract publishResponse = JsonConvert.DeserializeObject<FitbitPublishResponseContract>(responseString);
            await this.DeleteSteps(publishResponse.activityLog.logId);
            await this.TryPublish(steps);
        }

        private async Task DeleteSteps(long logId)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(string.Format(FitbitDataProvider.DeleteStepsUrl, logId)),
                Method = HttpMethod.Post,
            };

            request.Headers.Add(FitbitDataProvider.AuthHeader, this.CreateAuthHeader(Secrets.FitbitAuthToken));
            await client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> TryPublish(int steps)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(string.Format(FitbitDataProvider.PublishStepsUrl, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), steps)),
                Method = HttpMethod.Post,
            };

            request.Headers.Add(FitbitDataProvider.AuthHeader, this.CreateAuthHeader(Secrets.FitbitAuthToken));

            return  await client.SendAsync(request);            
        }

        public async Task<FitbitGetStepsDataContract> GetSteps()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(string.Format(FitbitDataProvider.GetStepsForDayUrl, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))),
                Method = HttpMethod.Get,
            };

            request.Headers.Add(FitbitDataProvider.AuthHeader, this.CreateAuthHeader(Secrets.FitbitAuthToken));

            HttpResponseMessage response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();

            FitbitGetStepsDataContract steps = JsonConvert.DeserializeObject<FitbitGetStepsDataContract>(result);
            return steps;
        }

        private string CreateAuthHeader(string accessToken)
        {
            return string.Concat(FitbitDataProvider.AuthTokenType, " ", accessToken);
        }
    }
}