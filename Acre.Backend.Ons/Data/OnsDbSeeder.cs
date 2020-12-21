using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using Acre.Backend.Ons.Models.Configurations;
using Acre.Backend.Ons.Services.Parsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Acre.Backend.Ons.Data
{
    public interface IOnsDbSeeder {
        Task SeedDatabase();
    }

    public class OnsDbSeeder : IOnsDbSeeder
    {
        private readonly OnsDbContext _ctx;
        private readonly IParserContext _parserContext;
        private readonly DatasetConfig _datasetConfig;
        private readonly ILogger<OnsDbSeeder> _logger;
        public OnsDbSeeder(OnsDbContext ctx, IParserContext parserContext, IOptions<DatasetConfig> datasetConfig, ILogger<OnsDbSeeder> logger) {
            _ctx = ctx;
            _parserContext = parserContext;
            _datasetConfig = datasetConfig.Value;
            _logger = logger;
        }

        public async Task SeedDatabase()
        {
            await ClearDatabase();
            var folder = _datasetConfig.FolderName;
            foreach(var dataset in _datasetConfig.Ons) {
                _parserContext.SetStrategy(dataset.DocumentType);
                await _parserContext.Parse($"{folder}/{dataset.FileName}");
            }
        }

        private async Task ClearDatabase() {
            _ctx.Categories.RemoveRange(_ctx.Categories);
            _ctx.Subcategories.RemoveRange(_ctx.Subcategories);
            _ctx.OnsByAge.RemoveRange(_ctx.OnsByAge);
            _ctx.OnsByComposition.RemoveRange(_ctx.OnsByComposition);
            _ctx.OnsByRegion.RemoveRange(_ctx.OnsByRegion);
            _ctx.Regions.RemoveRange(_ctx.Regions);
            await _ctx.SaveChangesAsync();
        }
    }
}