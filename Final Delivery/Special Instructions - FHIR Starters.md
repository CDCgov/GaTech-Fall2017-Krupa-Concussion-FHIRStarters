# Concussion and Traumatic Brain Injury in Pediatric Patients: A CDS App

This app is a physician-facing web-app designed to provide clinical decision support when diagnosing mTBI and generating management plans for pediatric patients.

## Instructions for Running the App

The following instructions will install the app and all dependencies by utilizing Docker. The app currently uses sample data on a standalone FHIR server, but with future work the app will be launchable from within an EHR system using SMART on FHIR. SMART on FHIR is already included in the app code.

1.	Install docker from <https://www.docker.com> (if not already installed).
2.	Verify docker is running
3.	Download this repo.
4.	Open a command prompt window (In Windows, click on Start Menu, go to “run”, type “cmd” and hit enter)
5.	In the command window, navigate to the directory GaTech-Fall2017-Krupa-Concussion-FHIRStarters
6.	Type `docker-compose up` in the command window and hit enter
7.	Wait for everything to build and startup (this may take a few minutes)
8.	In a web browser, navigate to <http://localhost:8083>

Observe that sample data is automatically pulled from the FHIR server using smart-on-fhir.

You can change the data that was pulled from the FHIR server or enter new data on this page. When all data is entered satisfactorily, click on the Submit button to be brought to the Results page.  From the Results page, you can either Generate a Management Plan or return to the Data Collection page to edit data or enter new data.  
