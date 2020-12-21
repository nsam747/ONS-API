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
    public class DatasetParserRegion : BaseParser
    {
        private readonly OnsDbContext _ctx;
        private readonly IParserCache _parserCache;
        private readonly ILogger<DatasetParserRegion> _logger;
        public override DocumentType[] SupportedDocumentTypes => new DocumentType[] { DocumentType.Region };
        public DatasetParserRegion(OnsDbContext ctx, ILogger<DatasetParserRegion> logger, IParserCache parserCache) {
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

                            // Create Regions
                            var regions = new List<Region>();
                            for(int i = startColumn; i <= endColumn; i++) {
                                var region = sheet.Cells[1,i].Text;
                                regions.Add(new Region { Name = region });
                            }
                            await _ctx.Regions.AddRangeAsync(regions);

                            var categories = new Stack<Category>();
                            var newCategories = new List<Category>();
                            var onsByRegion = new List<OnsByRegion>();
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
                                    var onsByRegions = new List<OnsByRegion>();
                                    for(int j = 3; j < endColumn; j++) {
                                        var value = sheet.Cells[i, j].Text;
                                        if(!string.IsNullOrWhiteSpace(value) && Decimal.TryParse(value, out var decimalValue) || Decimal.TryParse(sheet.Cells[i + 1, j].Text, out decimalValue)) {
                                            // On the topic of "sheet.Cells[i + 1, j].Text"
                                            // Sometimes the value is one row below due to a multiline category title
                                            // i.e 4.3 Water supply and miscellaneous services in ONSByRegion2019.xlxs
                                            var entry = new OnsByRegion { Region = regions[j - startColumn], Value = decimalValue };
                                            subcategory.OnsByRegion.Add(entry);
                                            regions[j - startColumn].OnsByRegion.Add(entry);
                                            onsByRegions.Add(entry);
                                        }
                                    }
                                    await _ctx.OnsByRegion.AddRangeAsync(onsByRegions);
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
    }
}