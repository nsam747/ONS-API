namespace Acre.Backend.Ons.Data.Entity
{
    public class OnsByAge : BaseOns
    {
        public int UpperBoundAge { get; set; }
        public int LowerBoundAge { get; set; }
        public override string TableName => "Ons_By_Age";
    }
}