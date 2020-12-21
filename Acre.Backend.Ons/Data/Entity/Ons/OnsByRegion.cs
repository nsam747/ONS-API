namespace Acre.Backend.Ons.Data.Entity
{
    public class OnsByRegion : BaseOns
    {
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public override string TableName => "Ons_By_Region";
    }
}