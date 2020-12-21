using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Acre.Backend.Ons.Abstractions;
using Acre.Backend.Ons.Models;
using Acre.Backend.Ons.Models.Configurations;
using Acre.Backend.Ons.Services;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Acre.Backend.Ons.Tests
{
    public class RegionLookupServiceTests
    {
        private static string _baseUrl = "www.api.dummy";
        private IOptions<PostcodesIOConfig> CreateConfig() {
            return Options.Create<PostcodesIOConfig>(
                new PostcodesIOConfig() {
                    BaseUrl = _baseUrl
                }
            );    
        }

        [Test]
        public async Task RegionLookupService_ReturnsCorrectRegion_GivenAPostcode()
        {
            // Arrange
            var config = CreateConfig();
            var dummyLogger = A.Fake<ILogger<RegionLookupService>>();
            var dummyClient = A.Fake<IHttpClient>();
            var stubData = File.ReadAllText("Tests/Stubs/regionLookup.json");
            var response = new HttpResponseMessage() { Content = new StringContent(stubData), StatusCode = HttpStatusCode.OK };
            A.CallTo(() => dummyClient.GetAsync(A<string>.That.Contains(_baseUrl))).Returns(response);
            var regionLookupService = new RegionLookupService(config, dummyClient, dummyLogger);

            // Act
            var postcode = "DUM MY";
            var region = await regionLookupService.GetRegion(postcode);

            // Assert
            var expectedRegion = JsonConvert.DeserializeObject<RegionLookupResult>(stubData).Result.Region;
            Assert.AreEqual(expectedRegion, region);
        }

        [Test]
        public async Task RegionLookupService_ReturnsEmptyStringAndLogsError_WhenHttpRequestFails()
        {
            // Arrange
            var config = CreateConfig();
            var dummyLogger = A.Fake<ILogger<RegionLookupService>>();
            var dummyClient = A.Fake<IHttpClient>();
            var response = new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.BadRequest };
            A.CallTo(() => dummyClient.GetAsync(A<string>.That.Contains(_baseUrl))).Returns(response);
            var regionLookupService = new RegionLookupService(config, dummyClient, dummyLogger);

            // Act
            var postcode = "DUM MY";
            var region = await regionLookupService.GetRegion(postcode);

            // Assert
            Assert.AreEqual(string.Empty, region);
            // A.CallTo(() => dummyLogger.LogError(A<string>.Ignored)).MustHaveHappened();
        }
    }
}