using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;

namespace Acre.Backend.Ons.Services.Parsers
{
    public interface IParserContext : IParser {
        void SetStrategy(DocumentType documentType);
    } 

    public class ParserContext : IParserContext {
        private readonly IEnumerable<IDatasetParser> _parsers;       
        private IDatasetParser _strategy { get; set; }
        public ParserContext(IEnumerable<IDatasetParser> parsers)
        {
            _parsers = parsers;
        }

        public async Task Parse(string filePath)
        {
            if(_strategy == null) throw new Exception("Cannot parse file without an active strategy");
            await _strategy.Parse(filePath);
        }

        public void SetStrategy(DocumentType documentType)
        {
            _strategy = _parsers.First(p => p.SupportedDocumentTypes.Contains(documentType));
            if(_strategy == null) throw new Exception($"Could not match DocumentType '{documentType}' to any registered {nameof(IDatasetParser)}s");
        }
    }
}