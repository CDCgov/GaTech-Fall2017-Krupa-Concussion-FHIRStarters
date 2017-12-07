using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;

namespace fhirStartersApp.Services
{
    public class ClinicalDecisionSupportService : IDecisionSupportService
    {
        private readonly IEnumerable<IRule> _ruleLibrary;

        public ClinicalDecisionSupportService(Assembly ruleAssembly)
        {
            var ruleInterface = typeof(IRule);
            var ruleTypes = ruleAssembly.GetTypes().Where(t => ruleInterface.IsAssignableFrom(t) && t.IsClass);
            _ruleLibrary = ruleTypes.Select(Activator.CreateInstance).Cast<IRule>();
        }

        public async Task<IEnumerable<DecisionSupportResult>> ProcessCollectedDataAsync(Patient patient)
        {
            var tasks = _ruleLibrary.Select(rule => rule.EvaluateAsync(patient));
            var resultResults = await Task.WhenAll(tasks);
            return resultResults.SelectMany(rr => rr);
        }
    }
}
