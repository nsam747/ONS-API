using System.ComponentModel.DataAnnotations.Schema;

namespace Acre.Backend.Ons.Data.Entity
{
    public abstract class BaseOns : BaseEntity
    {
        [ForeignKey("Subcategory")]
        public int SubcategoryId { get; set; }
        public virtual Subcategory Subcategory { get; set; }
        public decimal Value { get; set; }
        [NotMapped]
        public abstract string TableName { get; }
    }
}