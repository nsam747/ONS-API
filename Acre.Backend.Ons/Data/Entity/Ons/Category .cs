using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acre.Backend.Ons.Data.Entity
{
    public class Category : BaseCategory
    {
        public virtual List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    }
}