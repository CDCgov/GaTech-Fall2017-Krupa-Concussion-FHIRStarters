# CDS Values

## Table of Data Values

The following table lists data values needed by the CDS logic and their (potential) sources.

| Symbol                                                  | Type            | Source           | Notes                                           |
| ------------------------------------------------------- | --------------- | ---------------- | ----------------------------------------------- |
| Age                                                     | Number          | FHIR             | Age (years)                                     |
| GlasgowComaScale                                        | Number          | FHIR             | Glasgow Coma Scale                              |
| SignsOfAlteredMentalStatus                              | Trilean         | Interface?       | Altered mental status                           |
| SignsOfPalpableSkullFracture                            | Trilean         | Interface?       | Palpable skull fracture                         |
| LossOfConsciousness                                     | Trilean         | FHIR             | Loss of consciousness                           |
| LossOfConsciousnessTime                                 | Number          | FHIR             | Loss of consciousness (seconds)                 |
| OptScalpHematoma                                        | Trilean         | FHIR             | Occipital/parietal/temporal scalp hematoma      |
| SevereMechanismOfInjury                                 | Trilean         | Interface?       | Severe mechanism of injury                      |
| AbnormalBehaviorPerParentalAssessment                   | Trilean         | Interface        | Acting normally, according to parents           |
| SignsOfBasilarSkullFracture                             | Trilean         | FHIR?            | Basilar skull fracture                          |
| Vomiting                                                | Trilean         | FHIR?            | Vomiting                                        |
| AnyHeadaches                                            | Trilean         | FHIR             | Any headaches                                   |
| SevereHeadache                                          | Trilean         | FHIR?            | Severe headache                                 |
| WorseningSymptoms                                       | Trilean         | Interface        | Worsening symptoms                              |
| ConcussionHistory                                       | Trilean         | FHIR             | History of concussion(s)                        |
| CtImagingTaken                                          | Trilean         | FHIR             | Cranial CT image taken?                         |
| CtImagingResultsAbnormal                                | Trilean         | FHIR?            | Cranial CT image results are abnormal?          |
| LightNoiseSensitivity                                   | Trilean         | FHIR?            | Sensitivity to light/noise                      |
| MinorBluntHeadTraumaContusions                          | Trilean         | FHIR             | Cerebal contusions from minor blunt head trauma |
| MinorBluntHeadTraumaContusionsSmallAndIsolated          | Trilean         | FHIR?            | If contusions, small and isolated?              |
| IncludeSchoolRecommendations                            | Boolean         | Interface        | Include school recommendations in results?      |
| RestRecommended                                         | Boolean         | Interface        | Rest recommended?                               |
| RestRecommendedDays                                     | Number          | Interface        | Days of rest recommended                        |
| SignsOfOtherSkullFracture                               | Trilean         | FHIR?            | Other skull fracture                            |
| OtherScalpHematoma                                      | Trilean         | FHIR             | Other scalp hematoma                            |
| ChronicDiseases                                         | Trilean         | FHIR             | Chronic diseases (asthma, diabetes, etc)        |
| SevereSymptoms                                          | Trilean         | FHIR             | Severe symptoms                                 |
| PersistentMentalStatusChanges                           | Trilean         | FHIR?            | Persistent mental status changes                |
| CerebralContusionsSuspected                             | Trilean         | FHIR?            | Cerebral contusions                             |
| MriTaken                                                | Trilean         | FHIR             | MRI taken                                       |
| MriResultsAbnormal                                      | Trilean         | FHIR             | MRI results abnormal                            |
