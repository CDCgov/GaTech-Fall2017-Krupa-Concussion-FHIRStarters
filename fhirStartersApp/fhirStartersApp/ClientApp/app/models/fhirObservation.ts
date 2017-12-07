export interface IFhirObservation {
    code: ICode;
    valueQuantity: any;
    valueCodeableConcept: IValueCodableConcept;
    subject: any;
}

export interface ICode {
    coding: any;
    text: string;
}

export interface IValueCodableConcept {
    coding: Array<any>;
    text: string;
}