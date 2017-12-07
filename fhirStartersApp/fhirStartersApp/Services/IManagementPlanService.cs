using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;

namespace fhirStartersApp.Services
{
    public interface IManagementPlanService
    {
        ManagementPlan Generate(IEnumerable<DecisionSupportResult> decisionSupportResults);
    }
}
