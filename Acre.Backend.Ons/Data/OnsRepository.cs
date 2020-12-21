using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data.Entity;
using Acre.Backend.Ons.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acre.Backend.Ons.Data
{
    public interface IOnsRepository {
        Task<OnsResponseModel> GetOutgoingsModelByFilter(dynamic filter);
        // Task<OnsResponseModel> GetOutgoingsModelByAge(string region);
        Task<decimal> GetOutgoingBySubcategory(string subcategory, string region);
        Task<decimal> GetOutgoingBySubcategory(string[] subcategory, string region);
        Task<decimal> GetOutgoingBySubcategory(string subcategory, int age);
        Task<decimal> GetOutgoingBySubcategory(string[] subcategory, int age);
        Task<decimal> GetOutgoingByCategory(string category, string region);
        Task<decimal> GetOutgoingByCategory(string category, int age);
        // Task<decimal> GetOutgoingBySubcategory(string subcategory, int dependents, bool isEmployed);
        // Task<decimal> GetOutgoingByCategory(string category, int dependents, bool isEmployed);
    }

    public class OnsRepository : IOnsRepository
    {
        private readonly OnsDbContext _ctx;
        private readonly ILogger<OnsRepository> _logger;
        public OnsRepository(OnsDbContext ctx, ILogger<OnsRepository> logger) {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<OnsResponseModel> GetOutgoingsModelByFilter(dynamic filter) 
        {
            if(!(filter is string || filter is int)) throw new InvalidOperationException("Outgoings must be either filtered by Region {string} or Age {int}");
            var outgoing_water = GetOutgoingBySubcategory(SubcategoryConstants.Water, filter);
            var outgoings_communications = GetOutgoingByCategory(CategoryConstants.Communications, filter);
            var outgoings_mortgage_rent = GetOutgoingBySubcategory(SubcategoryConstants.MortgageRentCouncilTax, filter);;
            var outgoings_insurance = GetOutgoingBySubcategory(SubcategoryConstants.Insurance, filter);
            var outgoings_investments = GetOutgoingBySubcategory(SubcategoryConstants.Investments, filter);
            var outgoings_council_tax = GetOutgoingBySubcategory(SubcategoryConstants.CouncilTax, filter);
            var outgoings_food = GetOutgoingByCategory(CategoryConstants.Food, filter);
            var outgoings_clothing = GetOutgoingBySubcategory(SubcategoryConstants.Clothing, filter);
            var outgoings_other_living_costs = GetOutgoingByCategory(CategoryConstants.OtherLivingCosts, filter);
            var outgoings_entertainment = GetOutgoingBySubcategory(SubcategoryConstants.Entertainment, filter);
            var outgoings_holidays = GetOutgoingBySubcategory(SubcategoryConstants.Holidays, filter);
            // var outgoings_sports = GetOutgoingBySubcategory(SubcategoryConstants.Sports, filter);
            var outgoings_pension = GetOutgoingBySubcategory(SubcategoryConstants.Pension, filter);
            var outgoings_car_costs = GetOutgoingBySubcategory(SubcategoryConstants.CarCosts, filter);
            var outgoings_other_transport_costs = GetOutgoingBySubcategory(SubcategoryConstants.OtherTransportCosts, filter);
            // var outgoings_child_care = GetOutgoingBySubcategory(SubcategoryConstants.ChildCare, filter);
            var outgoings_fuel = GetOutgoingBySubcategory(SubcategoryConstants.Fuel, filter); 
            var outgoings_ground_rent_service_charge_shared_equity_rent = GetOutgoingBySubcategory(SubcategoryConstants.GroundRentServiceCharge, filter);
            // var outgoings_television_license = GetOutgoingBySubcategory(SubcategoryConstants.TelevisionLicense, filter);
            var outgoings_household_repairs = GetOutgoingBySubcategory(SubcategoryConstants.HouseholdRepairs, filter);
            var outgoings_additional_details = GetOutgoingByCategory(CategoryConstants.AdditionalDetails, filter);
            
            // Run all tasks in parallel
            await Task.WhenAll(new Task[]{ outgoing_water,
                outgoings_communications,
                outgoings_mortgage_rent,
                outgoings_insurance,
                outgoings_investments,
                outgoings_council_tax,
                outgoings_food,
                outgoings_clothing,
                outgoings_other_living_costs,
                outgoings_entertainment,
                outgoings_holidays,
                outgoings_pension,
                outgoings_car_costs,
                outgoings_other_transport_costs,
                outgoings_fuel,
                outgoings_ground_rent_service_charge_shared_equity_rent,
                outgoings_household_repairs,
                outgoings_additional_details
            });

            // All tasks will be completed by this point
            return new OnsResponseModel() {
                outgoings_water = await outgoing_water,
                outgoings_communications = await outgoings_communications,
                outgoings_mortgage_rent = await outgoings_mortgage_rent,
                outgoings_insurance = await outgoings_insurance,
                outgoings_investments = await outgoings_investments,
                outgoings_council_tax = await outgoings_council_tax,
                outgoings_food = await outgoings_food,
                outgoings_clothing = await outgoings_clothing,
                outgoings_other_living_costs = await outgoings_other_living_costs,
                outgoings_entertainment = await outgoings_entertainment,
                outgoings_holidays = await outgoings_holidays,
                outgoings_pension = await outgoings_pension,
                outgoings_car_costs = await outgoings_car_costs,
                outgoings_other_transport_costs = await outgoings_other_transport_costs,
                outgoings_fuel = await outgoings_fuel,
                outgoings_ground_rent_service_charge_shared_equity_rent = await outgoings_ground_rent_service_charge_shared_equity_rent,
                outgoings_household_repairs = await outgoings_household_repairs,
                outgoings_additional_details = await outgoings_additional_details
            };
        }

        public async Task<decimal> GetOutgoingByCategory(string category, string region)
        {
            var value = await _ctx.OnsByRegion.Where(ons => (ons.Region.Name == region) && ons.Subcategory.Category.Name == category).ToListAsync();
            if(value == null) _logger.LogError($"No {nameof(_ctx.OnsByRegion)} entry found for category '{category}' and region '{region}'");
            return value.Sum(ons => ons.Value);
        }

        public async Task<decimal> GetOutgoingByCategory(string category, int age)
        {
            var value = await _ctx.OnsByAge.Where(ons => (ons.UpperBoundAge >= age) && (ons.LowerBoundAge <= age) && ons.Subcategory.Category.Name == category).ToListAsync();
            if(value == null) _logger.LogError($"No {nameof(_ctx.OnsByRegion)} entry found for category '{category}' and age '{age}'");
            return value.Sum(ons => ons.Value);
        }

        public async Task<decimal> GetOutgoingBySubcategory(string[] subcategories, string region)
        {
            return (await Task.WhenAll(subcategories.Select(subcategory => GetOutgoingBySubcategory(subcategory, region)))).Sum();
        }

        public async Task<decimal> GetOutgoingBySubcategory(string subcategory, string region)
        {
            var value = await _ctx.OnsByRegion.Where(ons => (ons.Region.Name == region) && ons.Subcategory.Name == subcategory).FirstOrDefaultAsync();
            if(value == null) _logger.LogError($"No {nameof(_ctx.OnsByRegion)} entry found for subcategory '{subcategory}' and region '{region}'");
            return value.Value;
        }

        public async Task<decimal> GetOutgoingBySubcategory(string[] subcategories, int age)
        {
            return (await Task.WhenAll(subcategories.Select(subcategory => GetOutgoingBySubcategory(subcategory, age)))).Sum();
        }

        public async Task<decimal> GetOutgoingBySubcategory(string subcategory, int age)
        {
            var value = await _ctx.OnsByAge.Where(ons => (ons.UpperBoundAge >= age) && (ons.LowerBoundAge <= age) && ons.Subcategory.Name == subcategory).FirstOrDefaultAsync();
            if(value == null) _logger.LogError($"No {nameof(_ctx.OnsByRegion)} entry found for subcategory '{subcategory}' and age '{age}'");
            return value.Value;
        }
    }
}