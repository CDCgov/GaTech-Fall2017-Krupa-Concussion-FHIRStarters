using System;
using System.Linq;
using System.Threading.Tasks;
using fhirStartersApp.Data;
using fhirStartersApp.DecisionSupport;
using fhirStartersApp.DecisionSupport.MtbiRules;
using Xunit;

namespace fhirStartersApp.Test
{
    public class OrderCtImagingRuleTest
    {
        private const string CtRecommended = "CT Scan Recommended";
        private const string CtNotRecommended = "CT Scan NOT Recommended";
        private const string ObservationInHospital = "Observation in Hospital Recommended";

        private readonly OrderCtImagingRule _rule;
        private readonly Patient _patient;

        public OrderCtImagingRuleTest()
        {
            _rule = new OrderCtImagingRule();
            _patient = new Patient
            {
                Data = new Observations()
            };
        }

        [Fact]
        public async Task RequestGcsGivenNoGcs()
        {
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description == nameof(Observations.GlasgowComaScale));
        }

        [Theory]
        [InlineData(13)]
        [InlineData(14)]
        public async Task CtScanRecommendedGivenGcsOf13Or14(int gcs)
        {
            _patient.Data.GlasgowComaScale = gcs;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Fact]
        public async Task RequestAgeGivenGcs15AndNoAge()
        {
            _patient.Data.GlasgowComaScale = 15;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description == nameof(Patient.Age));
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task RequestAlteredMentalStatusOrPalpableSkullFractureGivenGcs15AndAgeLessThanTwo(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.SignsOfAlteredMentalStatus),
                nameof(Observations.SignsOfPalpableSkullFracture)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task RequestAlteredMentalStatusOrBasilarSkullFractureGivenGcs15AndAgeTwoOrGreater(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            var results = await _rule.EvaluateAsync(_patient);
            var expected = new[]
            {
                nameof(Observations.SignsOfAlteredMentalStatus),
                nameof(Observations.SignsOfBasilarSkullFracture)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task CtScanRecommendedGivenGcs15AgeLessThanTwoAndPalpableSkullFracture(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task CtScanRecommendedGivenGcs15AndAlteredMentalStatus(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task CtScanRecommendedGivenGcs15AgeTwoOrGreaterAndBasilarSkullFracture(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task MoreDataNeededGivenGcs15AgeLessThanTwoAndNoPalpableSkullFractureOrAlteredMentalStatus(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.OptScalpHematoma),
                nameof(Observations.LossOfConsciousness),
                nameof(Observations.SevereMechanismOfInjury),
                nameof(Observations.AbnormalBehaviorPerParentalAssessment)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task LocTimeNeededGivenGcs15AgeLessThanTwoAndNoPalpableSkullFractureOrAlteredMentalStatus(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.Yes;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.LossOfConsciousnessTime)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task MoreDataNeededGivenGcs15AgeTwoOrGreaterAndNoBasilarSkullFractureOrAlteredMentalStatus(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.LossOfConsciousness),
                nameof(Observations.Vomiting),
                nameof(Observations.SevereHeadache),
                nameof(Observations.SevereMechanismOfInjury)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task CtScanNotRecommendedGivenGcs15AgeTwoAndNoSymptoms(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtNotRecommended);
        }

        [Theory]
        [InlineData(12, 4)]
        [InlineData(23, 4)]
        [InlineData(12, 0)]
        [InlineData(23, 0)]
        public async Task CtScanNotRecommendedGivenGcs15AgeLessThanTwoAndLossOfConciousnessLessThan5Seconds(int ageInMonths, int secondsUnconcious)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.Yes;
            _patient.Data.LossOfConsciousnessTime = TimeSpan.FromSeconds(secondsUnconcious);
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);
            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtNotRecommended);
        }

        [Theory]
        [InlineData(3, 5)]
        [InlineData(4, 5)]
        [InlineData(23, 5)]
        [InlineData(3, 6)]
        [InlineData(4, 6)]
        [InlineData(23, 6)]
        public async Task WorseningSymptomsNeededGivenGcs15AgeLessThanTwoButGreaterThanThreeMonthsAndLossOfConciousnessGreaterThan5(int ageInMonths, int secondsUnconcious)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.Yes;
            _patient.Data.LossOfConsciousnessTime = TimeSpan.FromSeconds(secondsUnconcious);
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.WorseningSymptoms)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task WorseningSymptomsNeededGivenGcs15AgeTwoAndSevereHeadache(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            var expected = new[]
            {
                nameof(Observations.WorseningSymptoms)
            };
            Assert.Contains(results,
                x => x.Type == DecisionSupportResultType.MoreInformationRequired &&
                     x.Description.Split(',').Count(d => expected.Contains(d.Trim())) == expected.Length);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task CtScanRecommendedGivenGcs15AgeTwoAndMultipleFindings(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.Yes;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task CtScanRecommendedGivenGcs15AgeTwoAndWorseningSymptoms(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.Yes;
            _patient.Data.WorseningSymptoms = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task CtScanRecommendedGivenGcs15AgeLessThanTwoAndMultipleFindings(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.Yes;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task CtScanRecommendedGivenGcs15AgeLessThanTwoAndWorseningSymptoms(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.Yes;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            _patient.Data.WorseningSymptoms = Trilean.Yes;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(1)]
        public async Task CtScanRecommendedGivenGcs15AgeLessThanThreeMonths(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.Yes;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == CtRecommended);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(23)]
        public async Task ObservationInHospitalRecommendedGivenGcs15AgeLessThanTwoAndOneSymptom(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfPalpableSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.OptScalpHematoma = Trilean.Yes;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.AbnormalBehaviorPerParentalAssessment = Trilean.No;
            _patient.Data.WorseningSymptoms = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == ObservationInHospital);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(70)]
        public async Task ObservationInHospitalRecommendedGivenGcs15AgeTwoAndOneSymptom(int ageInMonths)
        {
            _patient.Data.GlasgowComaScale = 15;
            _patient.Age = new Age(ageInMonths, AgeUnit.Months);
            _patient.Data.SignsOfBasilarSkullFracture = Trilean.No;
            _patient.Data.SignsOfAlteredMentalStatus = Trilean.No;
            _patient.Data.LossOfConsciousness = Trilean.No;
            _patient.Data.Vomiting = Trilean.No;
            _patient.Data.SevereMechanismOfInjury = Trilean.No;
            _patient.Data.SevereHeadache = Trilean.Yes;
            _patient.Data.WorseningSymptoms = Trilean.No;
            var results = await _rule.EvaluateAsync(_patient);

            Assert.Single(results,
                x => x.Type == DecisionSupportResultType.ActionRecommendation &&
                     x.Description == ObservationInHospital);
        }
    }
}
