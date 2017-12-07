import { Component, Inject, OnInit } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Patient, DataSource, TriState, Age, AgeUnit, Timespan, TimeUnit } from '../../models/patient';
import { IFhirObservation } from '../../models/fhirObservation';
import { DecisionSupportResult, DecisionSupportResultType } from '../../models/decisionsupport';
import { FhirPatient } from '../../external/fhirclient';
import { AppService } from '../../app.service';
import { Router } from '@angular/router';


@Component({
    selector: 'datacollection',
    templateUrl: './datacollection.component.html',
    styleUrls: ['./datacollection.component.css']
})
export class DataCollectionComponent implements OnInit {
    public ageUnit = AgeUnit;
    public timeUnit = TimeUnit;

    public patient = new Patient();
    private results = new Array<DecisionSupportResult>();
    private smart: FhirPatient;
    public patientName: string;
    static readonly fhirBase = "baseDstu3"; 
    public someFHIR: boolean = false;
    public highlightFHIR: boolean = true;
    public loading = false;

    constructor(public appService: AppService,
        private readonly http: Http,
        @Inject('BASE_URL') private baseUrl: string,
        private router: Router) {
        const fhirUrl = (baseUrl.indexOf('localhost') > 0 ? baseUrl.substring(0, baseUrl.lastIndexOf(window.location.port)) + "8080/" : "https://fhirtesting.hdap.gatech.edu/ConcussionFhir/") + DataCollectionComponent.fhirBase;
        this.smart = new FhirPatient({
            serviceUrl: fhirUrl,
            patientId: "e58ee6f3-64ce-4f99-8383-ddc0584375dc",
            auth: {
                type: 'none'
            }
        });
        /* Create a patient welcome banner */
        const pq = this.smart.read();

        pq.then((p: any) => {
            var name = p.name[0];
            this.patientName = name.given + " " + name.family;
            if (p.birthDate) {
                this.patient.age.value = Age.fromBirthDate(p.birthDate);
                this.patient.age.source = DataSource.FHIR;
            }
        });

        const observations = this.smart.fetchAll({ type: "Observation" });
        observations.then((results: any) => {
            this.populateObservations(results);
        });

        const conditions = this.smart.search({ type: 'Condition' });
        conditions.then((c: any) => {
            //console.log(c);
        });
    }

    ngOnInit() {
        this.patient = this.appService.getPatient();
    }

    get resultsCount(): number {
        return this.results.length;
    }

    private populateObservations(observations: IFhirObservation[]): void {
        for (let observation of observations) {
            if (!observation.valueCodeableConcept || !observation.valueCodeableConcept.text) continue; //TODO: this might be incorrect depending on different observations

            const codeableConceptText = observation.valueCodeableConcept.text.toLowerCase();
            if (!this.patient.data.GlasgowComaScale.value && codeableConceptText.indexOf("glasgow") > -1) {
                const matches = codeableConceptText.match(/\d+$/);
                if (matches) {
                    this.patient.data.GlasgowComaScale.value = Number.parseInt(matches[0]);
                    this.patient.data.GlasgowComaScale.source = DataSource.FHIR;
                    //TODO: get the latest observation
                }
            } else if (this.patient.data.OptScalpHematoma.value === TriState.Unknown && codeableConceptText.indexOf("hematoma") > -1 && codeableConceptText.indexOf("scalp") > -1) {
                this.patient.data.OptScalpHematoma.value = TriState.Yes;
                this.patient.data.OptScalpHematoma.source = DataSource.FHIR;
                break;
            }
        }
    }

    public submit(): void {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        this.loading = true;
        this.http.post(this.baseUrl + 'api/DataCollection',
            this.patient.toJson(),
            { headers: headers })
            .subscribe(result => {
                const jsonResult = result.json();
                if (jsonResult) {
                    this.appService.clearRequiredFields();
                    jsonResult.map((jr: any) => DecisionSupportResult.fromJson(jr)).forEach((r: DecisionSupportResult) => {
                        this.results.push(r);
                        if (r.type === DecisionSupportResultType.MoreInformationRequired) {
                            for (let field of r.description.split(","))
                                this.appService.requireField(field);
                        }
                    });
                    this.appService.putData(this.patient, this.results);
                    this.router.navigate(['/results']);
                    this.loading = false;
                }
            },
            error => console.error(error));
    }

    public putData() {
        this.appService.putData(this.patient, this.results);
    }

    public setAgeUnit(val: AgeUnit, source = DataSource.Manual): void {
        if (this.patient.age == null || this.patient.age.value == null) {
            this.patient.age = {
                value: new Age(null, val),
                source: source
            };
        } else {
            this.patient.age.value.unit = val;
        }
    }

    public setLocTimeUnit(val: TimeUnit, source = DataSource.Manual): void {
        if (this.patient.data.LossOfConsciousnessTime == null || this.patient.data.LossOfConsciousnessTime.value == null) {
            this.patient.data.LossOfConsciousnessTime = {
                value: new Timespan(null, val),
                source: source
            }
        } else {
            this.patient.data.LossOfConsciousnessTime.value.unit = val;
        }
    }
}
