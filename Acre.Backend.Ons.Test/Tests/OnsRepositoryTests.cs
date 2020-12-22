using System.IO;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Data.Entity;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Acre.Backend.Ons.Tests
{
    public class OnsRepositoryTests
    {
        private OnsRepository _onsRepository;
        private async Task<OnsRepository> GetSeededRepository() {
            if(_onsRepository == null) {
                var dbContext = new OnsDbContext(new DbContextOptionsBuilder<OnsDbContext>().UseInMemoryDatabase("test").Options);
                var nestedData = JsonConvert.DeserializeObject<Category[]>(File.ReadAllText("Tests/Stubs/onsSeedData.json"));
                await dbContext.Categories.AddRangeAsync(nestedData);
                await dbContext.SaveChangesAsync();
                _onsRepository =  new OnsRepository(dbContext, A.Dummy<ILogger<OnsRepository>>());
            }
            return _onsRepository;
        }

        [Test]
        public async Task GetOutgoingBySubcategory_ReturnsOutgoings_BasedOnAge() {
            var onsRepository = await GetSeededRepository();

            var outgoingsA = await onsRepository.GetOutgoingBySubcategory("Subcategory1", 30);
            var outgoingsB = await onsRepository.GetOutgoingBySubcategory("Subcategory1", 50);
            var outgoingsC = await onsRepository.GetOutgoingBySubcategory("Subcategory1", 70);

            var outgoingsD = await onsRepository.GetOutgoingBySubcategory("Subcategory3", 30);
            var outgoingsE = await onsRepository.GetOutgoingBySubcategory("Subcategory3", 50);
            var outgoingsF = await onsRepository.GetOutgoingBySubcategory("Subcategory3", 70);

            Assert.AreEqual(200M, outgoingsA);
            Assert.AreEqual(300M, outgoingsB);
            Assert.AreEqual(400M, outgoingsC);

            Assert.AreEqual(400M, outgoingsD);
            Assert.AreEqual(600M, outgoingsE);
            Assert.AreEqual(800M, outgoingsF);
        }

        [Test]
        public async Task GetOutgoingBySubcategory_ReturnsOutgoings_BasedOnRegion() {
            var onsRepository = await GetSeededRepository();

            var outgoingsA = await onsRepository.GetOutgoingBySubcategory("Subcategory1", "Region1");
            var outgoingsB = await onsRepository.GetOutgoingBySubcategory("Subcategory1", "Region2");

            var outgoingsC = await onsRepository.GetOutgoingBySubcategory("Subcategory2", "Region3");
            var outgoingsD = await onsRepository.GetOutgoingBySubcategory("Subcategory2", "Region4");

            Assert.AreEqual(100, outgoingsA);
            Assert.AreEqual(200, outgoingsB);

            Assert.AreEqual(300, outgoingsC);
            Assert.AreEqual(400, outgoingsD);
        }

        [Test]
        public async Task GetOutgoingByCategory_ReturnsOutgoings_BasedOnAge() {
            var onsRepository = await GetSeededRepository();

            var outgoingsA = await onsRepository.GetOutgoingByCategory("Category1", 30);
            var outgoingsB = await onsRepository.GetOutgoingByCategory("Category2", 60);

            Assert.AreEqual(400M, outgoingsA);
            Assert.AreEqual(1200M, outgoingsB);
        }
    }
}