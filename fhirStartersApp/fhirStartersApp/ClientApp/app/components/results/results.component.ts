import { Component, Inject, OnInit } from '@angular/core';
import { Http, Headers, ResponseContentType } from '@angular/http';

import { Patient } from '../../models/patient';
import { DecisionSupportResult } from '../../models/decisionsupport';

import { AppService } from '../../app.service';
import { Router } from '@angular/router';

import * as FileSaver from 'file-saver';

@Component({
    selector: 'results',
    templateUrl: './results.component.html',
    styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit {
    public patient: Patient;
    public results: Array<DecisionSupportResult>;
    public loading = false;

    constructor(public appService: AppService, private readonly http: Http, @Inject('BASE_URL') private baseUrl: string, private router: Router) {

    }

    ngOnInit() {
        this.patient = this.appService.getPatient();
        this.results = this.appService.getResults();
    }

    goToDataCollection(): void {
        this.router.navigate(['/data-collection']);
    }

    generateManagementPlan(): void {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        this.loading = true;
        this.http.post(this.baseUrl + 'api/managementPlan',
                this.patient.toJson(),
                { headers: headers, responseType: ResponseContentType.Blob })
            .subscribe((response: any) => {
                const blob = response.blob();
                const filename = 'management-plan.docx';
                FileSaver.saveAs(blob, filename);
            });
    }
}
