using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using fhirStartersApp.Data;

namespace fhirStartersApp.DecisionSupport.MtbiRules
{
    public class OrderCtImagingRule : IRule
    {
        private const string RuleLabel = "Whether to order CT imaging";
        private const string CtRecommended = "CT Scan Recommended";
        private const string CtNotRecommended = "CT Scan NOT Recommended";
        private const string ObservationInHospital = "Observation in Hospital Recommended";
        private static readonly Age TwoYearsOfAge = new Age(2, AgeUnit.Years);
        private static readonly Age ThreeMonthsOfAge = new Age(3, AgeUnit.Months);

        public async Task<IEnumerable<DecisionSupportResult>> EvaluateAsync(Patient patient)
        {
            await Task.Delay(10); // TODO: remove this if other async methodology included
            var results = new List<DecisionSupportResult>();

            if (patient.Data.CtImagingTaken == Trilean.Yes)
            {
                results.Add(DecisionSupportResult.NoAction(RuleLabel));
                return results;
            }

            switch (patient.Data.GlasgowComaScale)
            {
                case 15:
                    // gcs is 15, so check age
                    return CheckAge(patient, results);
                case var gcs when gcs == 13 || gcs == 14:
                    results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtRecommended));
                    return results;
                case null:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(patient.Data.GlasgowComaScale)));
                    return results;
                default:
                    results.Add(DecisionSupportResult.NoAction(RuleLabel));
                    return results;
            }
        }

        private IEnumerable<DecisionSupportResult> CheckAge(Patient patient, ICollection<DecisionSupportResult> results)
        {
            switch (patient.Age)
            {
                case null:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Patient.Age)));
                    return results;
                case var age when age < TwoYearsOfAge:
                    return CheckMentalStatusOrPalpableSkullFracture(patient, results);
                default:
                    return CheckMentalStatusOrBasilarSkullFracture(patient, results);
            }
        }

        private IEnumerable<DecisionSupportResult> CheckMentalStatusOrPalpableSkullFracture(Patient patient,
            ICollection<DecisionSupportResult> results)
        {
            if (patient.Data.SignsOfAlteredMentalStatus == Trilean.Yes ||
                patient.Data.SignsOfPalpableSkullFracture == Trilean.Yes)
            {
                results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtRecommended));
                return results;
            }
            if (patient.Data.SignsOfAlteredMentalStatus == Trilean.Unknown ||
                patient.Data.SignsOfPalpableSkullFracture == Trilean.Unknown)
            {
                var dataNeeded = new List<string>();
                if (patient.Data.SignsOfAlteredMentalStatus == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SignsOfAlteredMentalStatus));
                if (patient.Data.SignsOfPalpableSkullFracture == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SignsOfPalpableSkullFracture));

                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                return results;
            }
            return CheckHematomaLocSevereInjuryOrAbnormalBehavior(patient, results);
        }

        private IEnumerable<DecisionSupportResult> CheckLocVomitingSevereInjuryOrHeadache(Patient patient,
            ICollection<DecisionSupportResult> results)
        {
            if (patient.Data.LossOfConsciousness == Trilean.Unknown ||
                patient.Data.Vomiting == Trilean.Unknown ||
                patient.Data.SevereMechanismOfInjury == Trilean.Unknown ||
                patient.Data.SevereHeadache == Trilean.Unknown)
            {
                var dataNeeded = new List<string>();
                if (patient.Data.LossOfConsciousness == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.LossOfConsciousness));
                if (patient.Data.Vomiting == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.Vomiting));
                if (patient.Data.SevereMechanismOfInjury == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SevereMechanismOfInjury));
                if (patient.Data.SevereHeadache == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SevereHeadache));

                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                return results;
            }

            var findings = new List<Trilean>
            {
                patient.Data.LossOfConsciousness,
                patient.Data.Vomiting,
                patient.Data.SevereMechanismOfInjury,
                patient.Data.SevereHeadache
            };

            var findingCount = findings.Count(finding => finding == Trilean.Yes);
            switch (findingCount)
            {
                case 0:
                    results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtNotRecommended));
                    return results;
                case 1:
                    return CheckWorseningSymptomsOrAge(patient, results);
                default:
                    results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtRecommended));
                    return results;
            }
        }

        private IEnumerable<DecisionSupportResult> CheckWorseningSymptomsOrAge(Patient patient,
            ICollection<DecisionSupportResult> results)
        {
            if (patient.Data.WorseningSymptoms == Trilean.Yes ||
                (!Equals(patient.Age, null) && patient.Age < ThreeMonthsOfAge))
            {
                results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtRecommended));
                return results;
            }
            if (patient.Data.WorseningSymptoms == Trilean.Unknown ||
                Equals(patient.Age, null))
            {
                var dataNeeded = new List<string>();
                if (patient.Data.WorseningSymptoms == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.WorseningSymptoms));
                if (Equals(patient.Age, null)) dataNeeded.Add(nameof(patient.Age));

                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                return results;
            }

            results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, ObservationInHospital));
            return results;
        }

        private IEnumerable<DecisionSupportResult> CheckMentalStatusOrBasilarSkullFracture(Patient patient,
            ICollection<DecisionSupportResult> results)
        {
            if (patient.Data.SignsOfAlteredMentalStatus == Trilean.Yes ||
                patient.Data.SignsOfBasilarSkullFracture == Trilean.Yes)
            {
                results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtRecommended));
                return results;
            }
            if (patient.Data.SignsOfAlteredMentalStatus == Trilean.Unknown ||
                patient.Data.SignsOfBasilarSkullFracture == Trilean.Unknown)
            {
                var dataNeeded = new List<string>();
                if (patient.Data.SignsOfAlteredMentalStatus == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SignsOfAlteredMentalStatus));
                if (patient.Data.SignsOfBasilarSkullFracture == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SignsOfBasilarSkullFracture));

                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                return results;
            }
            return CheckLocVomitingSevereInjuryOrHeadache(patient, results);
        }

        private IEnumerable<DecisionSupportResult> CheckHematomaLocSevereInjuryOrAbnormalBehavior(Patient patient,
            ICollection<DecisionSupportResult> results)
        {
            var findings = new List<bool>
            {
                patient.Data.OptScalpHematoma == Trilean.Yes,
                patient.Data.LossOfConsciousness == Trilean.Yes 
                && patient.Data.LossOfConsciousnessTime.HasValue 
                && patient.Data.LossOfConsciousnessTime.Value >= TimeSpan.FromSeconds(5),
                patient.Data.SevereMechanismOfInjury == Trilean.Yes,
                patient.Data.AbnormalBehaviorPerParentalAssessment == Trilean.Yes
            };

            var findingCount = findings.Count(finding => finding);
            switch (findingCount)
            {
                case 0:
                    if (patient.Data.OptScalpHematoma == Trilean.Unknown ||
                        patient.Data.LossOfConsciousness == Trilean.Unknown ||
                        (patient.Data.LossOfConsciousness == Trilean.Yes && patient.Data.LossOfConsciousnessTime == null) ||
                        patient.Data.SevereMechanismOfInjury == Trilean.Unknown ||
                        patient.Data.AbnormalBehaviorPerParentalAssessment == Trilean.Unknown)
                    {
                        var dataNeeded = new List<string>();
                        if (patient.Data.OptScalpHematoma == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.OptScalpHematoma));
                        if (patient.Data.LossOfConsciousness == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.LossOfConsciousness));
                        if (patient.Data.LossOfConsciousness == Trilean.Yes && patient.Data.LossOfConsciousnessTime == null) dataNeeded.Add(nameof(patient.Data.LossOfConsciousnessTime));
                        if (patient.Data.SevereMechanismOfInjury == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.SevereMechanismOfInjury));
                        if (patient.Data.AbnormalBehaviorPerParentalAssessment == Trilean.Unknown) dataNeeded.Add(nameof(patient.Data.AbnormalBehaviorPerParentalAssessment));

                        results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                        return results;
                    }
                    results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtNotRecommended));
                    return results;
                case 1:
                    return CheckWorseningSymptomsOrAge(patient, results);
                default:
                    results.Add(DecisionSupportResult.ActionRecommended(RuleLabel, CtRecommended));
                    return results;
            }
        }
    }
}
