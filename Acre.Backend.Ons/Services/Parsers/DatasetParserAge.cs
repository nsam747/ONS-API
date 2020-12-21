using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Acre.Backend.Ons.Services.Parsers
{
    public class DatasetParserAge : BaseParser
    {
        private readonly OnsDbContext _ctx;
        private readonly IParserCache _parserCache;
        private readonly ILogger<DatasetParserAge> _logger;
        public override DocumentType[] SupportedDocumentTypes => new DocumentType[] { DocumentType.Age };
        public DatasetParserAge(OnsDbContext ctx, ILogger<DatasetParserAge> logger, IParserCache parserCache) {
            _ctx = ctx;
            _logger = logger;
            _parserCache = parserCache;
        }
        public async override Task Parse(string filePath)
        {
            using(var transaction = await _ctx.Database.BeginTransactionAsync()) {
                _logger.LogInformation($"Created transaction for file: '{filePath}', beginning data parsing.");
                try {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using(var package = new ExcelPackage(new FileInfo(filePath))) {
                        foreach(var sheet in package.Workbook.Worksheets) {
                            var startRow = sheet.Dimension.Start.Row;
                            var endRow = sheet.Dimension.End.Row;
                            var startColumn = 5;
                            var endColumn = sheet.Dimension.End.Column;

                            // Read age ranges
                            var ageRanges = new List<AgeRange>();
                            for(int i = startColumn; i <= endColumn; i++) {
                                var ageRange = sheet.Cells[1,i].Text;
                                var ageRangeTuple = ParseAgeRange(ageRange);
                                ageRanges.Add(ageRangeTuple);
                            }

                            var categories = new Stack<Category>();
                            var newCategories = new List<Category>();
                            var onsByAge = new List<OnsByAge>();
                            for(int i = startRow + 1; i < endRow; i++) {
                                var sectionNumber = sheet.Cells[i,1].Text;
                                if(IsCategory(sectionNumber)) {
                                    // Create category
                                    var category = _parserCache.GetOrCreateCategory<Category>(sheet.Cells[i,2].Text, out var isCreated);
                                    if(isCreated) newCategories.Add(category);
                                    categories.Push(category);
                                } else if(IsSubcategory(sectionNumber)) {
                                    // Create subcategory
                                    var subcategory = _parserCache.GetOrCreateCategory<Subcategory>(sheet.Cells[i,2].Text, out var isCreated); 
                                    if(isCreated) categories.Peek().Subcategories.Add(subcategory);

                                    // Create database entries for each data value in subcategory
                                    var onsByAges = new List<OnsByAge>();
                                    for(int j = 3; j < endColumn; j++) {
                                        var value = sheet.Cells[i, j].Text;
                                        if(!string.IsNullOrWhiteSpace(value) && Decimal.TryParse(value, out var decimalValue) || Decimal.TryParse(sheet.Cells[i + 1, j].Text, out decimalValue)) {
                                            // On the topic of "sheet.Cells[i + 1, j].Text"
                                            // Sometimes the value is one row below due to a multiline category title
                                            // i.e 4.3 Water supply and miscellaneous services in ONSByAge2019.xlxs
                                            var ageRange = ageRanges[j - startColumn];
                                            if(ageRange != null) {
                                                var entry = new OnsByAge { 
                                                    UpperBoundAge = ageRange.UpperLimit, 
                                                    LowerBoundAge = ageRange.LowerLimit, 
                                                    Value = decimalValue 
                                                };
                                                subcategory.OnsByAge.Add(entry);
                                                onsByAges.Add(entry);
                                            }
                                        }
                                    }
                                    await _ctx.OnsByAge.AddRangeAsync(onsByAges);
                                } else {
                                    continue;
                                }
                            }
                            await _ctx.Categories.AddRangeAsync(newCategories);
                            var changes = await _ctx.SaveChangesAsync();
                            _logger.LogDebug($"Created {changes} entities for file '{filePath}'");
                            _logger.LogInformation($"Successfully parsed data for entities for file '{filePath}', committing transaction.");
                            await transaction.CommitAsync();
                        }
                    }
                } catch(Exception exc) {
                   _logger.LogError(exc, $"Exception occurred while parsing file '{filePath}' with {nameof(DatasetParserRegion)}");
                   await transaction.RollbackAsync();
                }
            }
        }

        private readonly Dictionary<string, AgeRange> _cache = new Dictionary<string, AgeRange>();
        private AgeRange ParseAgeRange(string ageRange)
        {
            if(string.IsNullOrWhiteSpace(ageRange)) return default(AgeRange);
            if(_cache.TryGetValue(ageRange, out var result)) return result;
            try {
                int upperLimit;
                int lowerLimit;
                if(ageRange.Contains('<')) {
                    upperLimit = int.Parse(ageRange.Replace("<", "")) - 1;
                    lowerLimit = 0;
                } else if(ageRange.Contains('+')) {
                    upperLimit = 200; // Should work until we start having people living past 200 :)
                    lowerLimit = int.Parse(ageRange.Replace("+", ""));
                } else if(ageRange.Contains('-')){
                    var split = ageRange.Split('-');
                    lowerLimit = int.Parse(split[0]);
                    upperLimit = int.Parse(split[1]);
                } else {
                    throw new FormatException();
                }
                var parsedAgeRange = new AgeRange { UpperLimit = upperLimit, LowerLimit = lowerLimit };
                _cache.Add(ageRange, parsedAgeRange);
                return parsedAgeRange;
            } catch (FormatException) {
                _logger.LogWarning($"Unable to parse age range value '{ageRange}', skipping.");
                return default(AgeRange);
            }
        }

        private class AgeRange {
            public int UpperLimit { get; set; }
            public int LowerLimit { get; set; }
        }
    }
}