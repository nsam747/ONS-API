using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acre.Backend.Ons.Data.Entity
{
    public class Region : BaseEntity
    {
        public string Name { get; set; }
        [InverseProperty("Region")]
        public virtual List<OnsByRegion> OnsByRegion { get; set; } = new List<OnsByRegion>();
    }
}