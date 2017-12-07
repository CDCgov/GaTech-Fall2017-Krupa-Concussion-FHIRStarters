using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;

namespace fhirStartersApp.DecisionSupport.MtbiRules
{
    public class DischargeRule : IRule
    {
        private const string RuleLabel = "Discharge instructions";
        public static readonly string SchoolRecommendations = $@"○ No physical activity during recess{Environment.NewLine}
○ No physical education (PE) class{Environment.NewLine}
○ No after-school sports{Environment.NewLine}
○ Shorten school day{Environment.NewLine}
○ Later school start time{Environment.NewLine}
○ Reduce the amount of homework{Environment.NewLine}
○ Postpone classroom tests or standardized testing{Environment.NewLine}
○ Provide extended time to complete school work, homework, or take tests{Environment.NewLine}
○ Provide written notes for school lessons and assignments (when possible){Environment.NewLine}
○ Allow for a quiet place to take rest breaks throughout the day{Environment.NewLine}
○ Lessen the amount of screen time for the student, such as computers, tablets, etc.";
        public const string DaysRestTemplate = "Patient should rest for {0} days before returning to school.";
        public const string HeadacheDrugs = "Give ibuprofen or acetaminophen to help with headache (as needed).";
        public const string SunglassesEarplugs = "Allow the patient to wear sunglasses, earplugs or headphones.";

        public async Task<IEnumerable<DecisionSupportResult>> EvaluateAsync(Patient patient)
        {
            await Task.Delay(10); // TODO: remove this if other async methodology included
            var results = new List<DecisionSupportResult>();

            if (!patient.Data.RestRecommended.HasValue)
            {
                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.RestRecommended)));
            }
            else
            {
                results.Add(
                    patient.Data.RestRecommendedDays.HasValue
                        ? ManagementPlanRecommendation(
                            string.Format(DaysRestTemplate, patient.Data.RestRecommendedDays))
                        : DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.RestRecommendedDays)));
            }

            if (patient.Data.SevereHeadache == Trilean.Yes)
            {
                results.Add(ManagementPlanRecommendation(HeadacheDrugs));
            }

            if (patient.Data.LightNoiseSensitivity == Trilean.Yes)
            {
                results.Add(ManagementPlanRecommendation(SunglassesEarplugs));
            }

            if (!patient.Data.IncludeSchoolRecommendations.HasValue)
            {
                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.IncludeSchoolRecommendations)));
            }
            else if (patient.Data.IncludeSchoolRecommendations.Value)
            {
                results.Add(ManagementPlanRecommendation(SchoolRecommendations));
            }

            if (!string.IsNullOrWhiteSpace(patient.Data.CustomDischargeInstructions))
            {
                results.Add(ManagementPlanRecommendation(patient.Data.CustomDischargeInstructions));
            }

            return results;
        }

        private static DecisionSupportResult ManagementPlanRecommendation(string recommendation)
        {
            return DecisionSupportResult.ManagementPlanRecommendation(RuleLabel, recommendation);
        }
    }
}
