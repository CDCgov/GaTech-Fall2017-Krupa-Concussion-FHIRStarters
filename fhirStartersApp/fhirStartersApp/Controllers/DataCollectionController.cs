using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;
using fhirStartersApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace fhirStartersApp.Controllers
{
    [Route("api/[controller]")]
    public class DataCollectionController : Controller
    {
        private readonly IDecisionSupportService _decisionSupportService;

        public DataCollectionController(IDecisionSupportService decisionSupportService)
        {
            _decisionSupportService = decisionSupportService;
        }

        [HttpPost]
        public async Task<IEnumerable<DecisionSupportResult>> Post([FromBody] Patient patient)
        {
            var decisionSupportResults = await _decisionSupportService.ProcessCollectedDataAsync(patient);
            return decisionSupportResults;
        }
    }
}
