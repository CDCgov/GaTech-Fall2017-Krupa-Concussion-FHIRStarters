using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;

namespace fhirStartersApp.Services
{
    public interface IDecisionSupportService
    {
        /// <summary>
        /// Calculates a set of decision support results from the data on the given patient.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        Task<IEnumerable<DecisionSupportResult>> ProcessCollectedDataAsync(Patient patient);

        //IEnumerable<DecisionSupportResult> ProcessCollectedData(IEnumerable<DataInput> collectedData);
    }
}
