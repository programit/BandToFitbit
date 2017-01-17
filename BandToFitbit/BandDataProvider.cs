using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class BandDataProvider
    {        
        private const string AuthTokenType = "Bearer";
        private const string AuthHeader = "Authorization";

        private const string GetHourlyStepsUrl = "https://api.microsofthealth.net/v1/me/Summaries/Hourly";
        private const string GetTokenUrl = "https://login.live.com/oauth20_token.srf";
        
        private const string TokenFile = "tokenFile";

        public BandTokenContract previousToken { get; private set; }

        public BandDataProvider()
        {

            if (File.Exists(BandDataProvider.TokenFile))
            {
                try
                {
                    using (Stream stream = File.Open(BandDataProvider.TokenFile, FileMode.Open))
                    using (BinaryReader reader = new BinaryReader(stream))
                    {

                        previousToken = new BandTokenContract()
                        {
                            requestDate = DateTime.FromFileTime(reader.ReadInt64()),
                            expires_in = reader.ReadInt32(),
                            access_token = reader.ReadString(),
                            refresh_token = reader.ReadString()
                     };
                    }
                }
                catch (Exception)
                {
                    File.Delete(BandDataProvider.TokenFile);
                }
            }
        }

        public async Task<BandDataContract> GetSteps()
        {
            // Don't risk the request taking too long that the token expires, instead invalidate a few minutes early. 
            string accessToken;
            if (this.previousToken == null)
            {
                accessToken = await this.GetAccessToken();
            }
            else
            {
                if (this.previousToken.requestDate + TimeSpan.FromSeconds(this.previousToken.expires_in) < DateTime.Now.AddMinutes(-5))
                {
                    accessToken = await this.UseRefreshToken();
                }
                else
                {
                    accessToken = this.previousToken.access_token;
                }
            }

            return await this.MakeRequest(accessToken);
        }

        private async Task<BandDataContract> MakeRequest(string accessToken)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(string.Concat(BandDataProvider.GetHourlyStepsUrl, "?startTime=", DateTime.Today.AddDays(-1).ToUniversalTime().ToString("o"), "&endTime=", DateTime.Today.AddMinutes(-1).ToUniversalTime().ToString("o"))),
                Method = HttpMethod.Get,                
            };

            request.Headers.Add(BandDataProvider.AuthHeader, this.CreateAuthHeader(accessToken));
            HttpResponseMessage response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BandDataContract>(result);
        }

        private async Task<string> UseRefreshToken()
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent($"grant_type=refresh_token&client_id={Secrets.BandClientId}&redirect_uri=https%3A%2F%2Fjohnmorman.com&refresh_token={this.previousToken.refresh_token}&client_secret={Secrets.BandClientSecret}", Encoding.ASCII, "application/x-www-form-urlencoded");

            DateTime now = DateTime.Now;
            HttpResponseMessage response = await client.PostAsync(BandDataProvider.GetTokenUrl, content);
            string result = await response.Content.ReadAsStringAsync();

            BandTokenContract tokenContract = JsonConvert.DeserializeObject<BandTokenContract>(result);
            tokenContract.requestDate = now;

            this.previousToken = tokenContract;
            this.WriteTokenToFile(tokenContract);

            return tokenContract.access_token;
        }

        private async Task<string> GetAccessToken()
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent($"grant_type=authorization_code&client_id={Secrets.BandClientId}&redirect_uri=https%3A%2F%2Fjohnmorman.com&code={Secrets.CurrentBandCode}&client_secret={Secrets.BandClientSecret}", Encoding.ASCII, "application/x-www-form-urlencoded");

            DateTime now = DateTime.Now;
            HttpResponseMessage response = await client.PostAsync(BandDataProvider.GetTokenUrl, content);
            string result = await response.Content.ReadAsStringAsync();

            BandTokenContract tokenContract = JsonConvert.DeserializeObject<BandTokenContract>(result);
            tokenContract.requestDate = now;

            this.previousToken = tokenContract;
            this.WriteTokenToFile(tokenContract);

            return tokenContract.access_token;
        }

        private string CreateAuthHeader(string accessToken)
        {
            return string.Concat(BandDataProvider.AuthTokenType, " ", accessToken);
        }

        private void WriteTokenToFile(BandTokenContract token)
        {            
            using (Stream stream = File.Open(BandDataProvider.TokenFile, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(token.requestDate.ToFileTime());
                writer.Write(token.expires_in);
                writer.Write(token.access_token);
                writer.Write(token.refresh_token);
            }
        }
    }
}