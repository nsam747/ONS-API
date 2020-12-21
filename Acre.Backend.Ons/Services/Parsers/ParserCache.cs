using System;
using System.Collections.Generic;
using System.Linq;
using Acre.Backend.Ons.Data.Entity;

namespace Acre.Backend.Ons.Services.Parsers
{
    public interface IParserCache {
        T GetOrCreateCategory<T>(string categoryName, out bool isCreated) where T : BaseCategory, new();
    } 

    public class ParserCache : IParserCache {
        private readonly Dictionary<string, BaseCategory> _cache = new Dictionary<string, BaseCategory>();
        private readonly object _lock = new object();
        public T GetOrCreateCategory<T>(string categoryName, out bool isCreated) where T : BaseCategory, new()
        {
            lock(_lock) {
                isCreated = false;
                if(_cache.TryGetValue(GetHashCode<T>(categoryName), out var category)) return (T)category;
                var newCategory = new T();
                newCategory.Name = categoryName;
                _cache.Add(GetHashCode<T>(categoryName), newCategory);
                isCreated = true;
                return newCategory;
            }
        }

        private string GetHashCode<T>(string categoryName) => typeof(T).Name + categoryName;
    }
}