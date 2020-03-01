using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using consulService.DTOs;
using consulService.Config;

namespace consulService.Services
{
    public class ConsulService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<ConsulService> logger;
        public ConsulService(ILogger<ConsulService> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.configuration = configuration;
        }
        public async Task<Consul> GetFromConsul(string key) {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format("{0}/v1/kv/{1}", configuration["Consul:URL"], key));

            var client = clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var t = await JsonSerializer.DeserializeAsync
                    <Consul>(responseStream);
                
                t.created = DateTime.Now;
                t.validUntil = DateTime.Now.AddSeconds(configuration["Consul:RefreshInSeconds"].ToInt());

                return t;
            }

            return null;
        }
    }
}