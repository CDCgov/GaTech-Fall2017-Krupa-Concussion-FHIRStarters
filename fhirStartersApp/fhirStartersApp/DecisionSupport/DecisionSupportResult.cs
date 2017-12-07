using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fhirStartersApp.DecisionSupport
{
    public enum DecisionSupportResultType
    {
        NoAction,
        Error,
        MoreInformationRequired,
        ActionRecommendation,
        ManagementPlanRecommendation
    }

    public class DecisionSupportResult
    {
        private DecisionSupportResult(DecisionSupportResultType type, string ruleLabel, string description)
        {
            Type = type;
            RuleLabel = ruleLabel;
            Description = description;
        }

        public string RuleLabel { get; }
        public DecisionSupportResultType Type { get; }
        public string Description { get; }

        public static DecisionSupportResult NoAction(string ruleLabel)
        {
            return new DecisionSupportResult(DecisionSupportResultType.NoAction, ruleLabel, null);
        }

        public static DecisionSupportResult ActionRecommended(string ruleLabel, string action)
        {
            return new DecisionSupportResult(DecisionSupportResultType.ActionRecommendation, ruleLabel, action);
        }

        public static DecisionSupportResult NeedMoreData(string ruleLabel, string property)
        {
            return NeedMoreData(ruleLabel, new[] {property});
        }

        public static DecisionSupportResult NeedMoreData(string ruleLabel, IEnumerable<string> properties)
        {
            return new DecisionSupportResult(DecisionSupportResultType.MoreInformationRequired, ruleLabel, string.Join(",", properties));
        }

        public static DecisionSupportResult ManagementPlanRecommendation(string ruleLabel,
            string managementPlanRecommendation)
        {
            return new DecisionSupportResult(DecisionSupportResultType.ManagementPlanRecommendation, ruleLabel,
                managementPlanRecommendation);
        }
    }
}
