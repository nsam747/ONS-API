using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using Acre.Backend.Ons.Models.Case;
using Acre.Backend.Ons.Models.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Acre.Backend.Ons.Data
{
    public interface ICaseRepository {
        Case GetById(Guid id);
    }

    public class CaseRepository : ICaseRepository
    {
        private readonly IEnumerable<Case> _cases;
        private readonly ILogger<CaseRepository> _logger;
        public CaseRepository(IOptions<DatasetConfig> config, ILogger<CaseRepository> logger) {
            var filePath = config.Value.CaseFilePath;
            try {
                var data = File.ReadAllText(filePath);
                var cases = JsonConvert.DeserializeObject<IEnumerable<Case>>(data, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                }); // https://www.newtonsoft.com/json/help/html/NamingStrategySnakeCase.htm
                _cases = cases;
                _logger = logger;
            } catch (Exception exc) {
                logger.LogError($"Failed to deserialize case data from file '{filePath}'. Please note that the {config.Value.CaseFileName} file must be a JSON array not a JSON object.");
                throw exc;
            }
        }

        public Case GetById(Guid id)
        {
            return _cases.FirstOrDefault(c => c.CaseId == id);
        }
    }
}