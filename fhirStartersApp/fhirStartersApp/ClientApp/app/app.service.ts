import { Injectable } from '@angular/core';

import { Patient, DataSource, TriState, Age, AgeUnit } from './models/patient';
import { DecisionSupportResult } from './models/decisionsupport';

@Injectable()
export class AppService {
    private patient: Patient;
    private results: Array<DecisionSupportResult>;
    private neededFields: Set<string>;

    constructor() {
        this.patient = new Patient();
        this.results = new Array<DecisionSupportResult>();
        this.neededFields = new Set<string>();
    }

    putData(patient: Patient, results: Array<DecisionSupportResult>): void {
        this.patient = patient;
        this.results = results;
    }

    getPatient(): Patient {
        return this.patient;
    }

    getResults(): Array<DecisionSupportResult> {
        return this.results;
    }

    requireField(field: string): void {
        this.neededFields.add(field);
    }

    clearRequiredFields(): void {
        this.neededFields.clear();
    }

    fieldRequired(field: string): boolean {
        return this.neededFields.has(field);
    }
}