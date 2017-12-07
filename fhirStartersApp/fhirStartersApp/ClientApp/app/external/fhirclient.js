import fhir from "fhirclient";

export var FhirPatient = (function() {

    function FhirPatient (config) {
        this.client = FHIR.client(config);
    }

    FhirPatient.prototype.conformance = function(query) {
    	return this.client.patient.conformance(query);
    };
    FhirPatient.prototype.document = function(query) {
    	return this.client.patient.document(query);
    };
    FhirPatient.prototype.profile = function(query) {
    	return this.client.patient.profile(query);
    };
    FhirPatient.prototype.transaction = function(query) {
    	return this.client.patient.transaction(query);
    };
    FhirPatient.prototype.history = function(query) {
    	return this.client.patient.history(query);
    };
    FhirPatient.prototype.typeHistory = function(query) {
    	return this.client.patient.typeHistory(query);
    };
    FhirPatient.prototype.resourceHistory = function(query) {
    	return this.client.patient.resourceHistory(query);
    };
    FhirPatient.prototype.read = function(query) {
    	return this.client.patient.read(query);
    };
    FhirPatient.prototype.read = function() {
        return this.client.patient.read();
    };
    FhirPatient.prototype.vread = function(query) {
    	return this.client.patient.vread(query);
    };
    FhirPatient.prototype.delete = function(query) {
    	return this.client.patient.delete(query);
    };
    FhirPatient.prototype.create = function(query) {
    	return this.client.patient.create(query);
    };
    FhirPatient.prototype.validate = function(query) {
    	return this.client.patient.validate(query);
    };
    FhirPatient.prototype.search = function(query) {
    	return this.client.patient.api.search(query);
    };
    FhirPatient.prototype.fetchAll = function (query) {
        return this.client.patient.api.fetchAll(query);
    };
    FhirPatient.prototype.update = function(query) {
    	return this.client.patient.update(query);
    };
    FhirPatient.prototype.nextPage = function(query) {
    	return this.client.patient.nextPage(query);
    };
    FhirPatient.prototype.prevPage = function(query) {
    	return this.client.patient.prevPage(query);
    };
    FhirPatient.prototype.resolve = function(query) {
    	return this.client.patient.resolve(query);
    };

    return FhirPatient;

}());

export var FhirClient = (function() {

    function FhirClient (config) {
        this.client = FHIR.client(config);
    }

    FhirClient.prototype.conformance = function(query) {
    	return this.client.conformance(query);
    };
    FhirClient.prototype.document = function(query) {
    	return this.client.document(query);
    };
    FhirClient.prototype.profile = function(query) {
    	return this.client.profile(query);
    };
    FhirClient.prototype.transaction = function(query) {
    	return this.client.transaction(query);
    };
    FhirClient.prototype.history = function(query) {
    	return this.client.history(query);
    };
    FhirClient.prototype.typeHistory = function(query) {
    	return this.client.typeHistory(query);
    };
    FhirClient.prototype.resourceHistory = function(query) {
    	return this.client.resourceHistory(query);
    };
    FhirClient.prototype.read = function(query) {
    	return this.client.read(query);
    };
    FhirClient.prototype.read = function() {
        return this.client.read();
    };
    FhirClient.prototype.vread = function(query) {
    	return this.client.vread(query);
    };
    FhirClient.prototype.delete = function(query) {
    	return this.client.delete(query);
    };
    FhirClient.prototype.create = function(query) {
    	return this.client.create(query);
    };
    FhirClient.prototype.validate = function(query) {
    	return this.client.validate(query);
    };
    FhirClient.prototype.search = function(query) {
    	return this.client.search(query);
    };
    FhirClient.prototype.fetchAll = function (query) {
        return this.client.fetchAll(query);
    };
    FhirClient.prototype.update = function(query) {
    	return this.client.update(query);
    };
    FhirClient.prototype.nextPage = function(query) {
    	return this.client.nextPage(query);
    };
    FhirClient.prototype.prevPage = function(query) {
    	return this.client.prevPage(query);
    };
    FhirClient.prototype.resolve = function(query) {
    	return this.client.resolve(query);
    };

    return FhirClient;

}());