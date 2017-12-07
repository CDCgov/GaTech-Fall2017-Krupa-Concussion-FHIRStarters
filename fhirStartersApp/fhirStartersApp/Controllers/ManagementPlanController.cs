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
    public class ManagementPlanController : Controller
    {
        private readonly IDecisionSupportService _decisionSupportService;
        private readonly IManagementPlanService _managementPlanService;

        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".xls", "application/vnd.ms-excel"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"}
        };

        public ManagementPlanController(IDecisionSupportService decisionSupportService, IManagementPlanService managementPlanService)
        {
            _decisionSupportService = decisionSupportService;
            _managementPlanService = managementPlanService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Patient patient)
        {
            var decisionSupportResults = await _decisionSupportService.ProcessCollectedDataAsync(patient);
            var managementPlan = _managementPlanService.Generate(decisionSupportResults);
            return File(managementPlan.FileContents, GetContentType(managementPlan.FileExtension));
        }

        private static string GetContentType(string fileExtension)
        {
            var ext = fileExtension;
            if (!ext.StartsWith('.'))
            {
                ext = '.' + ext;
            }
            ext = ext.ToLowerInvariant();
            return MimeTypes[ext];
        }
    }
}