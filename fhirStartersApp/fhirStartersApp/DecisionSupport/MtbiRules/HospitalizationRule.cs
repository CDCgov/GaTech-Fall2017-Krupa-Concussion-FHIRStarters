using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.Utilities;

namespace fhirStartersApp.DecisionSupport.MtbiRules
{
    public class HospitalizationRule : IRule
    {
        private const string RuleLabel = "Whether to hospitalize";
        private const string HospitalizationRecommended = "Hospitalization Recommended";
        private const string HospitalizationUnnecessary = "Hospitalization generally unnecessary. Telephone/mail follow-up to assess for subsequent neuroimaging, neurologic complications, or neurosurgical intervention";
        private const string MriRecommended = "MRI Recommended";

        public async Task<IEnumerable<DecisionSupportResult>> EvaluateAsync(Patient patient)
        {
            await Task.Delay(10); // TODO: remove this if other async methodology included
            var results = new List<DecisionSupportResult>();

            if ((patient.Data.MriTaken == Trilean.Yes && patient.Data.MriResultsAbnormal == Trilean.Yes)
                || patient.Data.SevereSymptoms == Trilean.Yes
                || patient.Data.ChronicDiseases == Trilean.Yes
                || (patient.Data.GlasgowComaScale.HasValue && patient.Data.GlasgowComaScale.Value <= 13)
                || (patient.Data.CtImagingTaken == Trilean.Yes && patient.Data.CtImagingResultsAbnormal == Trilean.Yes))
            {
                results.Add(Hospitalize());
                return results;
            }

            // skull fracture and hematoma are single findings, so flatten all the possible results for each into a single count
            var findings = new Dictionary<string, Trilean>
            {
                { nameof(Observations.ConcussionHistory), patient.Data.ConcussionHistory },
                { nameof(Observations.SignsOfAlteredMentalStatus), patient.Data.SignsOfAlteredMentalStatus },
                { nameof(Observations.LossOfConsciousness), patient.Data.LossOfConsciousness },
                { nameof(Observations.Vomiting), patient.Data.Vomiting },
                { nameof(Observations.SevereHeadache), patient.Data.SevereHeadache },
                { nameof(Observations.SevereMechanismOfInjury), patient.Data.SevereMechanismOfInjury },
                { nameof(Observations.AbnormalBehaviorPerParentalAssessment), patient.Data.AbnormalBehaviorPerParentalAssessment }
            };
            var skullFractureFindings = new Dictionary<string, Trilean>
            {
                { nameof(Observations.SignsOfBasilarSkullFracture), patient.Data.SignsOfBasilarSkullFracture },
                { nameof(Observations.SignsOfPalpableSkullFracture), patient.Data.SignsOfPalpableSkullFracture },
                { nameof(Observations.SignsOfOtherSkullFracture), patient.Data.SignsOfOtherSkullFracture }
            };
            var hematomaFindings = new Dictionary<string, Trilean>
            {
                { nameof(Observations.OptScalpHematoma), patient.Data.OptScalpHematoma },
                { nameof(Observations.OtherScalpHematoma), patient.Data.OtherScalpHematoma }
            };

            var findingCount = findings.Count(finding => finding.Value == Trilean.Yes);
            if (skullFractureFindings.Any(sff => sff.Value == Trilean.Yes)) findingCount++;
            if (hematomaFindings.Any(hf => hf.Value == Trilean.Yes)) findingCount++;
            if (findingCount >= 3)
            {
                results.Add(Hospitalize());
                return results;
            }
            
            var dataNeeded = new List<string>();
            dataNeeded.AddRange(findings.Merge(skullFractureFindings).Merge(hematomaFindings)
                .Where(finding => finding.Value == Trilean.Unknown).Select(fm => fm.Key));
            if (patient.Data.SevereSymptoms == Trilean.Unknown) dataNeeded.Add(nameof(Observations.SevereSymptoms));
            if (patient.Data.ChronicDiseases == Trilean.Unknown)
                dataNeeded.Add(nameof(Observations.ChronicDiseases));
            if (patient.Data.MriTaken == Trilean.Yes && patient.Data.MriResultsAbnormal == Trilean.Unknown)
                dataNeeded.Add(nameof(Observations.MriResultsAbnormal));

            if (dataNeeded.Count > 0)
            {
                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                return results;
            }

            switch (patient.Data.CtImagingTaken)
            {
                case Trilean.Unknown:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.CtImagingTaken)));
                    return results;
                case Trilean.Yes:
                    return CtImagingTaken(patient);
                case Trilean.No:
                    return CtImagingNotTaken(patient);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<DecisionSupportResult> CtImagingTaken(Patient patient)
        {
            var results = new List<DecisionSupportResult>();
            switch (patient.Data.CtImagingResultsAbnormal)
            {
                case Trilean.Unknown:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.CtImagingResultsAbnormal)));
                    return results;
                case Trilean.Yes:
                    results.Add(Hospitalize());
                    return results;
                case Trilean.No:
                    return CerebralContusionsFromBluntTraumaCheck(patient);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<DecisionSupportResult> CtImagingNotTaken(Patient patient)
        {
            return PersistentMentalStatusCheck(patient);
        }

        private IEnumerable<DecisionSupportResult> CerebralContusionsFromBluntTraumaCheck(Patient patient)
        {
            var results = new List<DecisionSupportResult>();
            switch (patient.Data.MinorBluntHeadTraumaContusions)
            {
                case Trilean.Unknown:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.MinorBluntHeadTraumaContusions)));
                    return results;
                case Trilean.Yes:
                    return ContusionsSmallAndIsolatedCheck(patient);
                case Trilean.No:
                    return PersistentMentalStatusCheck(patient);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<DecisionSupportResult> ContusionsSmallAndIsolatedCheck(Patient patient)
        {
            var results = new List<DecisionSupportResult>();
            switch (patient.Data.MinorBluntHeadTraumaContusionsSmallAndIsolated)
            {
                case Trilean.Unknown:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.MinorBluntHeadTraumaContusionsSmallAndIsolated)));
                    return results;
                case Trilean.Yes:
                    return PersistentMentalStatusCheck(patient);
                case Trilean.No:
                    results.Add(Hospitalize());
                    return results;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<DecisionSupportResult> PersistentMentalStatusCheck(Patient patient)
        {
            var results = new List<DecisionSupportResult>();
            if (patient.Data.PersistentMentalStatusChanges == Trilean.Yes ||
                patient.Data.CerebralContusionsSuspected == Trilean.Yes)
            {
                return MriCheck(patient);
            }

            var dataNeeded = new List<string>();
            if (patient.Data.PersistentMentalStatusChanges == Trilean.Unknown)
                dataNeeded.Add(nameof(Observations.PersistentMentalStatusChanges));
            if (patient.Data.CerebralContusionsSuspected == Trilean.Unknown)
                dataNeeded.Add(nameof(Observations.CerebralContusionsSuspected));

            if (dataNeeded.Count > 0)
            {
                results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, dataNeeded));
                return results;
            }

            results.Add(DoNotHospitalize());
            return results;
        }

        private IEnumerable<DecisionSupportResult> MriCheck(Patient patient)
        {
            var results = new List<DecisionSupportResult>();

            switch (patient.Data.MriTaken)
            {
                case Trilean.Unknown:
                case Trilean.No:
                    results.AddRange(MriRecommendedResults());
                    return results;
                case Trilean.Yes:
                    return MriResultsCheck(patient);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<DecisionSupportResult> MriResultsCheck(Patient patient)
        {
            var results = new List<DecisionSupportResult>();
            switch (patient.Data.MriResultsAbnormal)
            {
                case Trilean.Unknown:
                    results.Add(DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.MriResultsAbnormal)));
                    break;
                case Trilean.Yes:
                    results.Add(Hospitalize());
                    break;
                case Trilean.No:
                    results.Add(DoNotHospitalize());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return results;
        }

        private static DecisionSupportResult Hospitalize()
        {
            return DecisionSupportResult.ActionRecommended(RuleLabel, HospitalizationRecommended);
        }

        private static DecisionSupportResult DoNotHospitalize()
        {
            return DecisionSupportResult.ActionRecommended(RuleLabel, HospitalizationUnnecessary);
        }

        private static IEnumerable<DecisionSupportResult> MriRecommendedResults()
        {
            var results = new DecisionSupportResult[2];
            results[0] = DecisionSupportResult.ActionRecommended(RuleLabel, MriRecommended);
            results[1] = DecisionSupportResult.NeedMoreData(RuleLabel, nameof(Observations.MriTaken));
            return results;
        }
    }
}
