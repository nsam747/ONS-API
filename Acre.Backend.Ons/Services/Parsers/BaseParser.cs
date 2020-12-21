using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using OfficeOpenXml;

namespace Acre.Backend.Ons.Services.Parsers
{
    public abstract class BaseParser : IDatasetParser
    {
        public abstract DocumentType[] SupportedDocumentTypes { get; }

        public abstract Task Parse(string filePath);

        private static Regex CategoryRegex = new Regex(@"^\d+$");
        protected virtual bool IsCategory(string cellText) {
            return CategoryRegex.Match(cellText).Success;
        }

        private static Regex SubcategoryRegex = new Regex(@"^\d+\.\d+$");
        protected virtual bool IsSubcategory(string cellText) {
            return SubcategoryRegex.Match(cellText).Success;
        }  
    }
}