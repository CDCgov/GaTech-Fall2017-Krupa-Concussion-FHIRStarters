using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fhirStartersApp.Data
{
    public class Observations
    {
        public Trilean ConcussionHistory { get; set; }
        public Trilean CtImagingTaken { get; set; }
        public Trilean CtImagingResultsAbnormal { get; set; }
        public Trilean MriTaken { get; set; }
        public Trilean MriResultsAbnormal { get; set; }
        public int? GlasgowComaScale { get; set; }
        public Trilean SignsOfAlteredMentalStatus { get; set; }
        public Trilean SignsOfPalpableSkullFracture { get; set; }
        public Trilean SignsOfBasilarSkullFracture { get; set; }
        public Trilean LossOfConsciousness { get; set; }
        public Trilean Vomiting { get; set; }
        public Trilean SevereMechanismOfInjury { get; set; }
        public Trilean SevereHeadache { get; set; }
        public Trilean WorseningSymptoms { get; set; }
        public Trilean LightNoiseSensitivity { get; set; }
        public Trilean SevereSymptoms { get; set; }
        /// <summary>
        /// (Occipital, Parietal, or Temporal)
        /// </summary>
        public Trilean OptScalpHematoma { get; set; }
        public TimeSpan? LossOfConsciousnessTime { get; set; }
        public Trilean AbnormalBehaviorPerParentalAssessment { get; set; }
        public Trilean ChronicDiseases { get; set; }
        public Trilean MinorBluntHeadTraumaContusions { get; set; }
        public Trilean MinorBluntHeadTraumaContusionsSmallAndIsolated { get; set; }
        public Trilean PersistentMentalStatusChanges { get; set; }
        public Trilean CerebralContusionsSuspected { get; set; }
        public Trilean SignsOfOtherSkullFracture { get; set; }
        public Trilean OtherScalpHematoma { get; set; }
        public bool? RestRecommended { get; set; }
        public int? RestRecommendedDays { get; set; }
        public bool? IncludeSchoolRecommendations { get; set; }
        public string CustomDischargeInstructions { get; set; }
    }


    /// <summary>
    /// https://en.wikipedia.org/wiki/Three-valued_logic
    /// </summary>
    public enum Trilean
    {
        Unknown,
        Yes,
        No
    }
}
