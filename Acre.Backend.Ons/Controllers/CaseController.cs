using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Models;
using Acre.Backend.Ons.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Acre.Backend.Ons.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class CaseController : ControllerBase
    {
        private readonly ILogger<CaseController> _logger;
        private readonly IOnsRepository _onsRepository;
        private readonly IRegionLookupService _regionLookupService;
        private readonly ICaseRepository _caseRepository;
        public CaseController(IOnsRepository onsRepository, ICaseRepository caseRepository, IRegionLookupService regionLookupService, ILogger<CaseController> logger)
        {
            _logger = logger;
            _regionLookupService = regionLookupService;
            _onsRepository = onsRepository;
            _caseRepository = caseRepository;
        }

        /// <summary>
        /// Calculates the various expenditures of the household associated with a given Case using their region.
        /// </summary>
        /// <param name="id">The Case Id to be queried against.</param>
        /// <param name="useAge">A flag to indicate whether or not age should be taken into account for this Case.
        /// The result will be averaged between that found for both ages and region when set to "true".</param>
        /// <returns>A model detailing the various predicted outgoings in GBP decimals.</returns>
        /// <response code="400">The associated Case could not be found.</response>
        /// <response code="500">The associated Case contains household members occupying more than one address.</response>
        [HttpGet("{id:Guid}/outgoings/by/region")]
        public async Task<ActionResult<OnsResponseModel>> OutgoingsByRegion(Guid id, [FromQuery]bool useAge)
        {
            _logger.LogInformation($"Request for CaseId {id} received by {nameof(CaseController)}.{nameof(OutgoingsByRegion)}.");
            var associatedCase = _caseRepository.GetById(id);
            if(associatedCase != null) {
                _logger.LogInformation($"Found associated case for {id}.");
                var region = await _regionLookupService.GetRegion(associatedCase.AssociatedPostcode);
                if(!associatedCase.HasVaryingPostcodes) {
                    var onsModel = await _onsRepository.GetOutgoingsModelByFilter(region);
                    if(useAge) {
                        var averagedResultByAgeOnsModel = await GetAverageOnsOutgoingsByAge(associatedCase.AssociatedAges);
                        return Ok((onsModel + averagedResultByAgeOnsModel) / 2);
                    }
                    else return Ok(onsModel);
                } else {
                    return BadRequest($"Case {id} has members occupying multiple postcodes: {string.Join(',', associatedCase.AssociatedPostcodes)}.");
                }
            } else {
                return NotFoundCaseResponse(id);
            }
        }

        /// <summary>
        /// Calculates the various expenditures of the household associated with a given Case using their ages.
        /// In the case of a household with adults of ages within varying age-range categories then the estimate is calculated for each member and then averaged.
        /// </summary>
        /// <param name="id">The Case Id to be queried against.</param>
        /// <returns>A model detailing the various predicted outgoings in GBP decimals.</returns>
        /// <response code="400">The associated Case could not be found.</response>
        [HttpGet("{id:Guid}/outgoings/by/age")]
        public async Task<ActionResult<OnsResponseModel>> OutgoingsByAge(Guid id)
        {
            _logger.LogInformation($"Request for CaseId {id} received by {nameof(CaseController)}.{nameof(OutgoingsByAge)}.");
            var associatedCase = _caseRepository.GetById(id);
            if(associatedCase != null) {
                _logger.LogInformation($"Found associated case for {id}.");
                var ages = associatedCase.AssociatedAges;
                if(associatedCase.HasVariousAges) {
                    var averagedResultOnsModel = GetAverageOnsOutgoingsByAge(ages);
                    return Ok(averagedResultOnsModel);
                } else {
                    var onsModel = await _onsRepository.GetOutgoingsModelByFilter(ages.First());
                    return Ok(onsModel);
                }
            } else {
                return NotFoundCaseResponse(id);
            }
        }

        private ActionResult<OnsResponseModel> NotFoundCaseResponse(Guid id) {
            _logger.LogWarning($"Could not find associated case for {id}.");
            return NotFound($"No case associated with id: {id} could be found.");
        }

        private async Task<OnsResponseModel> GetAverageOnsOutgoingsByAge(IEnumerable<int> ages) {
            var onsModels = await Task.WhenAll(ages.Select(age =>  _onsRepository.GetOutgoingsModelByFilter(age)));
            var averagedResultOnsModel = onsModels.Aggregate((currentModel, nextModel) => currentModel + nextModel) / onsModels.Length;
            return averagedResultOnsModel;
        }
    }
}
