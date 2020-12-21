using System.Threading.Tasks;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;

namespace Acre.Backend.Ons.Services.Parsers
{
    public interface IParser {
        Task Parse(string filePath);
    } 
    public interface IDatasetParser : IParser {
        DocumentType[] SupportedDocumentTypes { get; }
    } 
}