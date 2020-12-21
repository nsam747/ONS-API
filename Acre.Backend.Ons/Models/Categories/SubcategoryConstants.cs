namespace Acre.Backend.Ons.Models
{
    public static class SubcategoryConstants
    {
        public readonly static string Water = "Water supply and miscellaneous services";
        public readonly static string MortgageRentCouncilTax = "Housing: mortgage interest payments,";
        public readonly static string Insurance = "Insurance";
        public readonly static string Investments = "Savings and investments";
        public readonly static string CouncilTax = MortgageRentCouncilTax;
        public readonly static string Clothing = "Clothing";
        public readonly static string Entertainment = "Recreational and cultural services";
        public readonly static string[] Holidays = new string[] { "Holiday spending", "Package holidays", "Accommodation services"};
        public readonly static string Sports = "Sports admissions, subscriptions, leisure class fees";
        public readonly static string Pension = "Life assurance, contributions to pension funds";
        public readonly static string[] CarCosts = new string[] {"Purchase of vehicles", "Operation of personal transport" };
        public readonly static string OtherTransportCosts = "Transport services";
        public readonly static string Fuel = "Electricity, gas and other fuels";
        public readonly static string TelevisionLicense = "TV, video, satellite rental, cable subscriptions";
        public readonly static string HouseholdRepairs = "Maintenance and repair of dwelling";
        public readonly static string GroundRentServiceCharge = "Actual rentals for housing";
    }
}