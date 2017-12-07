using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;
using fhirStartersApp.DecisionSupport.MtbiRules;
using Xunit;
using System.Linq;

namespace fhirStartersApp.Test
{
    public class HospitalizationRuleTest
    {
        private readonly HospitalizationRule _rule;
        private readonly Patient _patient;

        private const string HospitalizationRecommended = "Hospitalization Recommended";
        private const string HospitalizationUnnecessary = "Hospitalization generally unnecessary. Telephone/mail follow-up to assess for subsequent neuroimaging, neurologic complications, or neurosurgical intervention";
        private const string MriRecommended = "MRI Recommended";

        public HospitalizationRuleTest()
        {
            _rule = new HospitalizationRule();
            _patient = new Patient
            {
                Data = new Observations()
            };
        }

        [Theory]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.Yes)]
        [InlineData(Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No, Trilean.Yes)]
        [InlineData(Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No, Trilean.Yes, Trilean.No, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No)]
        public async Task HospitalizationRecommendedGivenThreeOrMoreSymptoms(Trilean concussionHistory,
            Trilean alteredMentalStatus, Trilean basilarSkullFracture, Trilean palpableSkullFracture,
            Trilean signsOfOtherSkullFracture,
            Trilean lossOfConciousness, Trilean vomiting,
            Trilean severeHeadache, Trilean severeMechanismOfInjury, Trilean optScalpHematoma, Trilean otherScalpHematoma,
            Trilean abnormalBehaviorPerParents)
        {
            _patient.Data.ConcussionHistory = concussionHistory;
            _patient.Data.SignsOfAlteredMentalStatus = alteredMentalStatus;
            _patient.Data.SignsOfBasilarSkullFracture = basilarSkullFracture;
            _patient.Data.SignsOfPalpableSkullFracture = palpableSkullFracture;
            _patient.Data.SignsOfOtherSkullFracture = signsOfOtherSkullFracture;
            _patient.Data.LossOfConsciousness = lossOfConciousness;
            _patient.Data.Vomiting = vomiting;
            _patient.Data.SevereHeadache = severeHeadache;
            _patient.Data.SevereMechanismOfInjury = severeMechanismOfInjury;
            _patient.Data.OptScalpHematoma = optScalpHematoma;
            _patient.Data.OtherScalpHematoma = otherScalpHematoma;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = abnormalBehaviorPerParents;

            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        [Fact]
        public async Task MoreInformationNeededGivenNoInformation()
        {
            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.ConcussionHistory),
                nameof(Observations.SignsOfAlteredMentalStatus),
                nameof(Observations.SignsOfBasilarSkullFracture),
                nameof(Observations.SignsOfPalpableSkullFracture),
                nameof(Observations.SignsOfOtherSkullFracture),
                nameof(Observations.LossOfConsciousness),
                nameof(Observations.Vomiting),
                nameof(Observations.SevereHeadache),
                nameof(Observations.SevereMechanismOfInjury),
                nameof(Observations.OptScalpHematoma),
                nameof(Observations.OtherScalpHematoma),
                nameof(Observations.AbnormalBehaviorPerParentalAssessment),
                nameof(Observations.SevereSymptoms),
                nameof(Observations.ChronicDiseases)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(Trilean.Unknown, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No)]
        [InlineData(Trilean.Yes, Trilean.Unknown, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No)]
        [InlineData(Trilean.Yes, Trilean.No, Trilean.Unknown, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.Yes, Trilean.No, Trilean.Unknown, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Unknown, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Unknown, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Unknown, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Unknown, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Unknown, Trilean.Yes, Trilean.Yes, Trilean.Yes)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No, Trilean.Unknown, Trilean.Yes, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.No, Trilean.No, Trilean.Unknown, Trilean.No)]
        [InlineData(Trilean.No, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.Yes, Trilean.No, Trilean.No, Trilean.No, Trilean.No, Trilean.Yes, Trilean.Yes, Trilean.Unknown)]
        public async Task MoreInformationNeededGivenPartialInformation(Trilean concussionHistory,
            Trilean alteredMentalStatus, Trilean basilarSkullFracture, Trilean palpableSkullFracture,
            Trilean signsOfOtherSkullFracture,
            Trilean lossOfConciousness, Trilean vomiting,
            Trilean severeHeadache, Trilean severeMechanismOfInjury, Trilean optScalpHematoma,
            Trilean otherScalpHematoma,
            Trilean abnormalBehaviorPerParents)
        {
            _patient.Data.ConcussionHistory = concussionHistory;
            _patient.Data.SignsOfAlteredMentalStatus = alteredMentalStatus;
            _patient.Data.SignsOfBasilarSkullFracture = basilarSkullFracture;
            _patient.Data.SignsOfPalpableSkullFracture = palpableSkullFracture;
            _patient.Data.SignsOfOtherSkullFracture = signsOfOtherSkullFracture;
            _patient.Data.LossOfConsciousness = lossOfConciousness;
            _patient.Data.Vomiting = vomiting;
            _patient.Data.SevereHeadache = severeHeadache;
            _patient.Data.SevereMechanismOfInjury = severeMechanismOfInjury;
            _patient.Data.OptScalpHematoma = optScalpHematoma;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = abnormalBehaviorPerParents;
            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.ConcussionHistory),
                nameof(Observations.SignsOfAlteredMentalStatus),
                nameof(Observations.SignsOfBasilarSkullFracture),
                nameof(Observations.SignsOfPalpableSkullFracture),
                nameof(Observations.SignsOfOtherSkullFracture),
                nameof(Observations.LossOfConsciousness),
                nameof(Observations.Vomiting),
                nameof(Observations.SevereHeadache),
                nameof(Observations.SevereMechanismOfInjury),
                nameof(Observations.OptScalpHematoma),
                nameof(Observations.OtherScalpHematoma),
                nameof(Observations.AbnormalBehaviorPerParentalAssessment),
                nameof(Observations.SevereSymptoms),
                nameof(Observations.ChronicDiseases)
            };
            var observationType = typeof(Observations);
            expected = expected.Where(e =>
                Trilean.Unknown == (Trilean) observationType.GetProperty(e).GetValue(_patient.Data)).ToArray();
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(Trilean.Unknown, Trilean.Unknown)]
        [InlineData(Trilean.No, Trilean.Unknown)]
        [InlineData(Trilean.Unknown, Trilean.No)]
        public async Task SevereSymptomsAndChronicDiseasesNeededGivenNegativeInformation(Trilean chronicDiseases, Trilean severeSymptoms)
        {
            _patient.Data.ConcussionHistory = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfOtherSkullFracture = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.No;
            _patient.Data.OtherScalpHematoma = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;

            _patient.Data.ChronicDiseases = chronicDiseases;
            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.SevereSymptoms),
                nameof(Observations.ChronicDiseases)
            };
            var observationType = typeof(Observations);
            expected = expected.Where(e =>
                Trilean.Unknown == (Trilean)observationType.GetProperty(e).GetValue(_patient.Data)).ToArray();
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }
            
        [Fact]
        public async Task HospitalizationRecommendedGivenSevereSymptoms()
        {
            _patient.Data.SevereSymptoms = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        [Fact]
        public async Task HospitalizationRecommendedGivenChronicDiseases()
        {
            _patient.Data.ChronicDiseases = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        [Theory]
        [InlineData(13)]
        [InlineData(12)]
        public async Task HospitalizationRecommendedGivenGcsLessThanOrEqualTo13(int gcs)
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = gcs;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        [Fact]
        public async Task CtImagingResultsNeededGivenCtImagingTaken()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;

            _patient.Data.CtImagingTaken = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.CtImagingResultsAbnormal)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(null)]
        public async Task HospitalizationRecommendedGivenAbnormalCtImaging(int? gcs)
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = gcs;

            _patient.Data.CtImagingTaken = Trilean.Yes;
            _patient.Data.CtImagingResultsAbnormal = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        [Fact]
        public async Task CerebralContusionsFromMinorBluntHeadTraumaNeededGivenNormalCtImaging()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.Yes;
            _patient.Data.CtImagingResultsAbnormal = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.MinorBluntHeadTraumaContusions)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task CerebralContusionsSmallNeededGivenMinorBluntTraumaContusions()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.Yes;
            _patient.Data.CtImagingResultsAbnormal = Trilean.No;
            _patient.Data.MinorBluntHeadTraumaContusions = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.MinorBluntHeadTraumaContusionsSmallAndIsolated)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task HospitalizationRecommendedGivenContusionsNotSmallAndIsolated()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.Yes;
            _patient.Data.CtImagingResultsAbnormal = Trilean.No;
            _patient.Data.MinorBluntHeadTraumaContusions = Trilean.Yes;
            _patient.Data.MinorBluntHeadTraumaContusionsSmallAndIsolated = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        [Fact]
        public async Task PersistentMentalStatusChangesAndContusionsSuspectedNeededGivenContusionsSmallAndIsolated()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.Yes;
            _patient.Data.CtImagingResultsAbnormal = Trilean.No;
            _patient.Data.MinorBluntHeadTraumaContusions = Trilean.Yes;
            _patient.Data.MinorBluntHeadTraumaContusionsSmallAndIsolated = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.PersistentMentalStatusChanges),
                nameof(Observations.CerebralContusionsSuspected)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task PersistentMentalStatusChangesAndContusionsSuspectedNeededGivenNoContusionsFromBluntTrauma()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.Yes;
            _patient.Data.CtImagingResultsAbnormal = Trilean.No;
            _patient.Data.MinorBluntHeadTraumaContusions = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.PersistentMentalStatusChanges),
                nameof(Observations.CerebralContusionsSuspected)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task PersistentMentalStatusChangesAndContusionsSuspectedNeededGivenNoCtTaken()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.PersistentMentalStatusChanges),
                nameof(Observations.CerebralContusionsSuspected)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task PersistentMentalStatusChangesNeededGivenNoContusionsSuspected()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;
            _patient.Data.CerebralContusionsSuspected = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.PersistentMentalStatusChanges)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task ContusionsSuspectedNeededGivenNoPersistentMentalStatusChanges()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;
            _patient.Data.PersistentMentalStatusChanges = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.CerebralContusionsSuspected)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task HospitalizationUnnecessaryGivenNoPersistentMentalStatusChangesOrContusionsSuspected()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;
            _patient.Data.PersistentMentalStatusChanges = Trilean.No;
            _patient.Data.CerebralContusionsSuspected = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationUnnecessary);
        }

        [Fact]
        public async Task MriRecommendedGivenPersistentMentalStatusChanges()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;
            _patient.Data.PersistentMentalStatusChanges = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.MriTaken)
            };
            Assert.Collection(results,
                result => Assert.True(result.Type == DecisionSupportResultType.ActionRecommendation &&
                          result.Description == MriRecommended),
                result => Assert.True(result.Type == DecisionSupportResultType.MoreInformationRequired &&
                           result.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length));
        }

        [Fact]
        public async Task MriRecommendedGivenContusionsSuspected()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;
            _patient.Data.CerebralContusionsSuspected = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.MriTaken)
            };
            Assert.Collection(results,
                result => Assert.True(result.Type == DecisionSupportResultType.ActionRecommendation &&
                                      result.Description == MriRecommended),
                result => Assert.True(result.Type == DecisionSupportResultType.MoreInformationRequired &&
                                      result.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length));
        }

        [Fact]
        public async Task MriResultsNeededGivenMriTaken()
        {
            _patient.Data.MriTaken = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.MriResultsAbnormal)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Fact]
        public async Task HospitalizationUnnecessaryGivenNormalMri()
        {
            PassToGcs();

            _patient.Data.GlasgowComaScale = null;
            _patient.Data.CtImagingTaken = Trilean.No;
            _patient.Data.CerebralContusionsSuspected = Trilean.Yes;
            _patient.Data.MriTaken = Trilean.Yes;
            _patient.Data.MriResultsAbnormal = Trilean.No;

            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationUnnecessary);
        }

        [Fact]
        public async Task HospitalizationRecommendedGivenAbnormalMri()
        {
            _patient.Data.MriTaken = Trilean.Yes;
            _patient.Data.MriResultsAbnormal = Trilean.Yes;

            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == HospitalizationRecommended);
        }

        private void PassToGcs()
        {
            _patient.Data.ConcussionHistory = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfOtherSkullFracture = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.No;
            _patient.Data.OtherScalpHematoma = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            _patient.Data.SevereSymptoms = Trilean.No;
            _patient.Data.ChronicDiseases = Trilean.No;
        }
    }
}
