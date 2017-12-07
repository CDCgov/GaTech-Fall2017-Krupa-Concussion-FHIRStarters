# Project Overview

## The Problem
Concussion is a well-known type of traumatic brain injury and is also known as mild traumatic brain injury (mTBI).  Research suggests that patients with mTBI can present with different symptoms, which makes care and management of children with mTBI challenging. In addition, healthcare providers don't necessarily have the time or training to systematically assess and manage patients with suspected mTBI. The CDC reports that rates of emergency department visits of individuals aged 19 or younger presented with TBI doubled between 2001 and 2012.  Therefore, the problem is: how to improve *evidence-based diagnosis* and management of mTBI in these patients?

In general, as described by our mentors, there are two components to improving the diagnosis and management of mTBI:

* Improve the ability of healthcare providers to diagnose mTBI at the time of injury by using evidence-based guidelines, and
* Improve communication between clinicians, families, and schools in order to improve post-injury management


## The Solution
Our mentors identified the need for intelligent clinical decision support software to help facilitate these components. This project therefore aims to address the first component by developing a provider interface which provides clinical decision support based on input from the web interface and automatically-obtained information from integration with the provider's EHR via FHIR.


## Notable Features
* Extensible CDS rule structure
  * Several CDS rules have been implemented
  * Rule framework makes implementing new rules plug-and-play
* Responsive front-end design
  * Data retrieved from the EHR using SMART on FHIR are highlighted for the physician
* Compiles to Docker image for easy deployment
  * docker-compose file creates containers for the web application, a MySQL database for persisting application state, and FHIR server to simulate an EHR system
