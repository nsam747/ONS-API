using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Acre.Backend.Ons.Services.Parsers
{
    public class DatasetParserComposition : BaseParser
    {
        private readonly OnsDbContext _ctx;
        private readonly IParserCache _parserCache;
        private readonly ILogger<DatasetParserComposition> _logger;
        public override DocumentType[] SupportedDocumentTypes => new DocumentType[] { DocumentType.Composition };
        public DatasetParserComposition(OnsDbContext ctx, ILogger<DatasetParserComposition> logger, IParserCache parserCache) {
            _ctx = ctx;
            _parserCache = parserCache;
            _logger = logger;
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

                            // Create Column Number to Composition mappings
                            var columnCompositionMapping = new Dictionary<int, Composition>() {
                                // Retired
                                [6] = new Composition(adultCount: 1, dependantCount: 0, employmentStatus: EmploymentStatus.Pension),
                                [7] = new Composition(adultCount: 2, dependantCount: 0, employmentStatus: EmploymentStatus.Pension),
                                [8] = new Composition(adultCount: 1, dependantCount: 0, employmentStatus: EmploymentStatus.Retired),
                                [9] = new Composition(adultCount: 2, dependantCount: 0, employmentStatus: EmploymentStatus.Retired),
                                
                                // Non Retired
                                [10] = new Composition(adultCount: 1, dependantCount: 0, employmentStatus: EmploymentStatus.Employed),
                                [11] = new Composition(adultCount: 2, dependantCount: 0, employmentStatus: EmploymentStatus.Employed),

                                [12] = new Composition(adultCount: 1, dependantCount: 1, employmentStatus: EmploymentStatus.Employed),
                                [13] = new Composition(adultCount: 1, dependantCount: 2, employmentStatus: EmploymentStatus.Employed),

                                // Either
                                [14] = new Composition(adultCount: 1, dependantCount: 1, employmentStatus: EmploymentStatus.Any),
                                [15] = new Composition(adultCount: 1, dependantCount: 2, employmentStatus: EmploymentStatus.Any),

                                [16] = new Composition(adultCount: 2, dependantCount: 1, employmentStatus: EmploymentStatus.Any),
                                [17] = new Composition(adultCount: 2, dependantCount: 2, employmentStatus: EmploymentStatus.Any),
                                [18] = new Composition(adultCount: 2, dependantCount: 3, employmentStatus: EmploymentStatus.Any),

                                [19] = new Composition(adultCount: 3, dependantCount: 1, employmentStatus: EmploymentStatus.Any),
                                [20] = new Composition(adultCount: 3, dependantCount: 2, employmentStatus: EmploymentStatus.Any),
                            };
                        
                            var categories = new Stack<Category>();
                            var newCategories = new List<Category>();
                            var onsByComposition = new List<OnsByComposition>();
                            for(int i = startRow + 1; i < endRow; i++) {
                                var sectionNumber = sheet.Cells[i,1].Text;
                                if(IsCategory(sectionNumber)) {
                                    // Create category
                                    var categoryName = sheet.Cells[i,2].Text;
                                    var category = _parserCache.GetOrCreateCategory<Category>(categoryName, out var isCreatedCategory);
                                    if(isCreatedCategory) newCategories.Add(category);
                                    categories.Push(category);

                                    // Create a subcategory to map to the category since this document type doesn't have actual subcategories 
                                    var subcategory = _parserCache.GetOrCreateCategory<Subcategory>(categoryName, out var isCreatedSubcategory); 
                                    if(isCreatedSubcategory) categories.Peek().Subcategories.Add(subcategory);

                                    // Create database entries for each data value in subcategory
                                    var onsByCompositions = new List<OnsByComposition>();
                                    for(int j = startColumn; j < endColumn; j++) {
                                        var value = sheet.Cells[i, j].Text;
                                        if(!string.IsNullOrWhiteSpace(value) && Decimal.TryParse(value, out var decimalValue)) {
                                            var composition = columnCompositionMapping[j];
                                            var entry = new OnsByComposition { AdultCount = composition.AdultCount, DependantCount = composition.DependantCount, EmploymentStatus = composition.EmploymentStatus, Value = decimalValue };
                                            subcategory.OnsByComposition.Add(entry);
                                            onsByCompositions.Add(entry);
                                        }
                                    }
                                    await _ctx.OnsByComposition.AddRangeAsync(onsByCompositions);
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

        private class Composition {
            public Composition(int adultCount, int dependantCount, EmploymentStatus employmentStatus) {
                AdultCount = adultCount;
                DependantCount = dependantCount;
                EmploymentStatus = employmentStatus;
            }
            public int AdultCount { get; set; }
            public int DependantCount { get; set; }
            public EmploymentStatus EmploymentStatus { get; set; }
        }
    }
}