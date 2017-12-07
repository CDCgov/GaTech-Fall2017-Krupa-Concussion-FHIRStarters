using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;

namespace fhirStartersApp.DecisionSupport
{
    internal interface IRule
    {
        /// <summary>
        /// Evaluates a rule given the data on the patient and generates one or more decision support results.
        /// At a minimum, the evaluation will provide a result that enumerates the data that is 
        /// required to evaluate completely.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        Task<IEnumerable<DecisionSupportResult>> EvaluateAsync(Patient patient);
    }
}
