using System;
using System.Net.Http;
using System.Threading.Tasks;
using Acre.Backend.Ons.Abstractions;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using Acre.Backend.Ons.Models.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Acre.Backend.Ons.Services
{
    public interface IRegionLookupService {
        Task<string> GetRegion(string postcode);
    } 

    public class RegionLookupService : IRegionLookupService
    {
        private readonly string _baseUrl;
        private readonly IHttpClient  _client;
        private readonly ILogger<RegionLookupService> _logger;

        public RegionLookupService(IOptions<PostcodesIOConfig> config, IHttpClient client, ILogger<RegionLookupService> logger) {
            _baseUrl = config.Value.BaseUrl;
            _client = client;
            _logger = logger;
        }

        public async Task<string> GetRegion(string postcode)
        {
            var uri = $"{_baseUrl}/postcodes/{postcode}";
            var response = await _client.GetAsync(uri);
            if(response.IsSuccessStatusCode) {
                var data = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<RegionLookupResult>(data);
                return responseObject.Result.Region;
            } else {
                _logger.LogError($"Bad status code {(int)response.StatusCode} received when calling {uri}");
                return string.Empty;
            }
        }
    }
}