# Architecture

## High-Level

* Server-side: ASP.NET (C#)
  * Clinical decision support logic
* Client-side: Angular, SMART on FHIR Javascript library
  * User interface: data collection and results display
* Database: MySQL
  * Securely stores patient data guiding decision logic

## CDS Logic

The CDS logic processing is handled on the server. In the server-side code, the <code>DataCollectionController</code> class provides methods which handle API requests from the client. The client app POSTs patient and observation data to the server which is sent by this object to the <code>ClinicalDecisionSupportService</code>. This service runs the CDS rules (classes implementing <code>IRule</code>), collects their results, and sends them to the client as <code>DecisionSupportResult</code> objects.

The data fields (observations) are listed below, under [CDS Values](../cdsvalues/README.md). These represent all data collected by the web app and consumed by the server-side CDS rules.

## Web-Based Client

The Angular-based client application consists of two main Angular components, representing the two client views: the <code>DataCollectionComponent</code> and the <code>ResultsComponent</code>. The former is displayed by default. Both components operate in one Angular module. Data is shared between these components using the singleton <code>AppService</code> which also tracks which data fields the CDS logic has identified as needed. In addition, several models mirror data structures used on the server.

The client application also includes the SMART on FHIR JavaScript library which is capable of querying data on a FHIR server such as an EHR. This provides the potential to pull some data needed by the CDS logic into the interface automatically.
