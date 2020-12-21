using System.ComponentModel.DataAnnotations.Schema;

namespace Acre.Backend.Ons.Data.Entity
{
    public class OnsByComposition : BaseOns
    {
        public int AdultCount { get; set; }
        public int DependantCount { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public override string TableName => "Ons_By_Composition";
    }

    public enum EmploymentStatus {
        Employed,
        Pension,
        Retired,
        Any
    }
}