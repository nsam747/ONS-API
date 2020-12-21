using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acre.Backend.Ons.Data.Entity
{
    public class Subcategory : BaseCategory
    {
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual List<OnsByAge> OnsByAge { get; set; } = new List<OnsByAge>();
        public virtual List<OnsByComposition> OnsByComposition { get; set; } = new List<OnsByComposition>();
        public virtual List<OnsByRegion> OnsByRegion { get; set; } = new List<OnsByRegion>();
    }
}