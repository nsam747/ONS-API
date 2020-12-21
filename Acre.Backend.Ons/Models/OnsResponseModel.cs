namespace Acre.Backend.Ons.Models
{
    public class OnsResponseModel
    {
        public decimal outgoings_water { get; set; }
        public decimal outgoings_communications { get; set; }
        public decimal outgoings_mortgage_rent { get; set; }
        public decimal outgoings_insurance { get; set; }
        public decimal outgoings_investments { get; set; }
        public decimal outgoings_council_tax { get; set; }
        public decimal outgoings_food { get; set; }
        public decimal outgoings_clothing { get; set; }
        public decimal outgoings_other_living_costs { get; set; }
        public decimal outgoings_entertainment { get; set; }
        public decimal outgoings_holidays { get; set; }
        // public decimal outgoings_sports { get; set; }
        public decimal outgoings_pension { get; set; }
        public decimal outgoings_car_costs { get; set; }
        public decimal outgoings_other_transport_costs { get; set; }
        // public decimal outgoings_child_care { get; set; }
        public decimal outgoings_fuel { get; set; }
        public decimal outgoings_ground_rent_service_charge_shared_equity_rent { get; set; } 
        // public decimal outgoings_television_license { get; set; }
        public decimal outgoings_household_repairs { get; set; }
        public decimal outgoings_additional_details { get; set; }

        public static OnsResponseModel operator +(OnsResponseModel a, OnsResponseModel b) {
            // Would usually use reflection to iterate over the available properties and average them but this is to save time
            a.outgoings_water = a.outgoings_water + b.outgoings_water;
            a.outgoings_communications = a.outgoings_communications + b.outgoings_communications;
            a.outgoings_mortgage_rent = a.outgoings_mortgage_rent + b.outgoings_mortgage_rent;
            a.outgoings_insurance = a.outgoings_insurance + b.outgoings_insurance;
            a.outgoings_investments = a.outgoings_investments + b.outgoings_investments;
            a.outgoings_council_tax = a.outgoings_council_tax + b.outgoings_council_tax;
            a.outgoings_food = a.outgoings_food + b.outgoings_food;
            a.outgoings_clothing = a.outgoings_clothing + b.outgoings_clothing;
            a.outgoings_other_living_costs = a.outgoings_other_living_costs + b.outgoings_other_living_costs;
            a.outgoings_entertainment = a.outgoings_entertainment + b.outgoings_entertainment;
            a.outgoings_holidays = a.outgoings_holidays + b.outgoings_holidays;
            a.outgoings_pension = a.outgoings_pension + b.outgoings_pension;
            a.outgoings_car_costs = a.outgoings_car_costs + b.outgoings_car_costs;
            a.outgoings_other_transport_costs = a.outgoings_other_transport_costs + b.outgoings_other_transport_costs;
            a.outgoings_fuel = a.outgoings_fuel + b.outgoings_fuel;
            a.outgoings_ground_rent_service_charge_shared_equity_rent = a.outgoings_ground_rent_service_charge_shared_equity_rent + b.outgoings_ground_rent_service_charge_shared_equity_rent;
            a.outgoings_household_repairs = a.outgoings_household_repairs + b.outgoings_household_repairs;
            a.outgoings_additional_details = a.outgoings_additional_details + b.outgoings_additional_details;
            return a;
        }

        public static OnsResponseModel operator /(OnsResponseModel a, int b) {
            // Would usually use reflection to iterate over the available properties and average them but this is to save time
            a.outgoings_water = a.outgoings_water / b;
            a.outgoings_communications = a.outgoings_communications / b;
            a.outgoings_mortgage_rent = a.outgoings_mortgage_rent / b;
            a.outgoings_insurance = a.outgoings_insurance / b;
            a.outgoings_investments = a.outgoings_investments / b;
            a.outgoings_council_tax = a.outgoings_council_tax / b;
            a.outgoings_food = a.outgoings_food / b;
            a.outgoings_clothing = a.outgoings_clothing / b;
            a.outgoings_other_living_costs = a.outgoings_other_living_costs / b;
            a.outgoings_entertainment = a.outgoings_entertainment / b;
            a.outgoings_holidays = a.outgoings_holidays / b;
            a.outgoings_pension = a.outgoings_pension / b;
            a.outgoings_car_costs = a.outgoings_car_costs / b;
            a.outgoings_other_transport_costs = a.outgoings_other_transport_costs / b;
            a.outgoings_fuel = a.outgoings_fuel / b;
            a.outgoings_ground_rent_service_charge_shared_equity_rent = a.outgoings_ground_rent_service_charge_shared_equity_rent / b;
            a.outgoings_household_repairs = a.outgoings_household_repairs / b;
            a.outgoings_additional_details = a.outgoings_additional_details / b;
            return a;
        }
    }
}