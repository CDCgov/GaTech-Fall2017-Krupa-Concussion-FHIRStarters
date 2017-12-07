export interface FhirApi {
    conformance(query: any): any;
    document(query: any): any;
    profile(query: any): any;
    transaction(query: any): any;
    history(query: any): any;
    typeHistory(query: any): any;
    resourceHistory(query: any): any;
    read(query: any): any;
    read(): any;
    vread(query: any): any;
    delete(query: any): any;
    create(query: any): any;
    validate(query: any): any;
    search(query: any): any;
    fetchAll(query: any): any;
    update(query: any): any;
    nextPage(query: any): any;
    prevPage(query: any): any;
    resolve(query: any): any;
}

export declare class FhirPatient implements FhirApi {
    private client: any;

    constructor(config: any);

    conformance(query: any): any;
    document(query: any): any;
    profile(query: any): any;
    transaction(query: any): any;
    history(query: any): any;
    typeHistory(query: any): any;
    resourceHistory(query: any): any;
    read(query: any): any;
    read(): any;
    vread(query: any): any;
    delete(query: any): any;
    create(query: any): any;
    validate(query: any): any;
    search(query: any): any;
    fetchAll(query: any): any;
    update(query: any): any;
    nextPage(query: any): any;
    prevPage(query: any): any;
    resolve(query: any): any;

}

export declare class FhirClient implements FhirApi {
    private client: any;

    constructor(config: any);

    conformance(query: any): any;
    document(query: any): any;
    profile(query: any): any;
    transaction(query: any): any;
    history(query: any): any;
    typeHistory(query: any): any;
    resourceHistory(query: any): any;
    read(query: any): any;
    read(): any;
    vread(query: any): any;
    delete(query: any): any;
    create(query: any): any;
    validate(query: any): any;
    search(query: any): any;
    fetchAll(query: any): any;
    update(query: any): any;
    nextPage(query: any): any;
    prevPage(query: any): any;
    resolve(query: any): any;

}
