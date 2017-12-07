## Installation

The following instructions will install the app and all dependencies by utilizing Docker. The app currently uses sample data on a standalone FHIR server, but with future work the app will be launchable from within an EHR system using SMART on FHIR. SMART on FHIR is already included in the app code.

 1. If you don't already have docker (<https://www.docker.com>) installed, do so.
 2. Clone this repo.
 3. In a command window, navigate to the directory CONCUSSION-AND-TRAUMATIC-BRAIN-INJURY-IN-PEDIATRIC-PATIENTS
 4. Run `docker-compose up`
 5. Wait for everything to build and startup.
 6. In a browser, navigate to <http://localhost:8083>
 7. Observe that sample data is automatically pulled from the FHIR server using smart-on-fhir.

## Development Environment Setup

 1. Download [Visual Studio 2017 Installer](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)
 2. In the installer, under **Workloads>Web & Cloud**, select **ASP.NET and web development** and **Data storage and processing**. Under **Individual Components**, make sure **.NET Framework 4.6.2** (SDK and targeting) is selected. Install the selected components.
 3. Download and install [MySQL](https://dev.mysql.com/downloads/installer/) (scroll down for the installer, no need to login, just say "No thanks"). During installation, choose a custom install and select the **MySQL Server** as well as the **Connector/NET** (and a client if you want one).
 4. Download and install [MySQL for Visual Studio](https://dev.mysql.com/downloads/windows/visualstudio/) (choose **Typical** install, again no need to login).
 5. Open the MySQL command line client or if you installed a client, open it. Run the follwing commands:
  - `CREATE DATABASE mtbicds`.  
  - `CREATE USER 'fhirstarter'@'localhost' IDENTIFIED BY '[insert some password]';`
  - `GRANT ALL PRIVILEGES ON mtbicds.* TO 'fhirstarter'@'localhost' WITH GRANT OPTION;`
  - `CREATE USER 'fhirstarter'@'%' IDENTIFIED BY '[insert some password]';`
  - `GRANT ALL PRIVILEGES ON mtbicds.* TO 'fhirstarter'@'%' WITH GRANT OPTION;`
 6. Update the **DefaultConnection** string in the **appSettings.Development.json** file in the fhirStartersApp directory to reflect the username and password you use.
 7. Start Visual Studio 2017 and open the **fhirStartersApp.sln** file and wait for dependencies to install. 
 8. Follow [these](https://dev.mysql.com/doc/visual-studio/en/visual-studio-making-a-connection.html) instructions for connecting to a MySQL database.
 9. Attempt to run the server. If you experience an error such as *The command "node node_modules/webpack/bin/webpack.js" exited with code 1*, you may need to install a newer version of [NodeJS](https://nodejs.org/en/) (e.g. 6.11).
