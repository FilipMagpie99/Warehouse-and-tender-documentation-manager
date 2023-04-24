# Warehouse-and-tender-documentation-manager
Przetargi Web Application
This is a .NET 6 web application for small businesses to manage tender documentation and warehouse records. The application is divided into three main folders:

• 'InzynierkaAPI': This folder contains the minimal API for the application. It includes the controllers, models, and services required to handle HTTP requests and responses.

• 'PrzetargiHTTP': This folder contains the frontend code of the application. It includes Blazor, HTML, CSS, and JavaScript files that handle user interactions and communicate with the backend API.

• 'InzynierkaAPITest': This folder contains the test project for the API. It includes integration tests for the controllers. 
## Features
The Przetargi web application comes with the following features:

• Tender Documentation Management: The application allows users to manage their tender documentation by uploading, downloading, and deleting files. The files are stored in Azure Blob Storage for easy access and scalability.

• Warehouse Records Management: The application allows users to manage their warehouse records by adding, editing, and deleting records. The records are stored in a database for easy retrieval and querying.

• KRS API Integration: The application integrates with the KRS API to allow users to find companies by their KRS number. This feature is useful when filling out data about companies.

• Notification System: The application allows users to receive notifications to email about incoming projects. 

## Demo
A demo of the application is available at https://przetargi.azurewebsites.net/
In order to access the application enter test user credentials login: User1, password: User1.
Please note that the demo may not include all the features mentioned above, and some features may be limited.

## Getting Started
To run the application on your local machine, follow these steps:

1. Clone this repository to your local machine.
2. Install the .NET 6 SDK and Visual Studio or Visual Studio Code.
3. Open the Przetargi.sln solution file in Visual Studio or Visual Studio Code.
4. Run the InzynierkaAPI project to start the backend API.
5. Run the Client - Przetargi HTTP project to start the frontend application.
6. Navigate to http://localhost:5000 to view the application

