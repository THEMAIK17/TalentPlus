# TalentPlus HR System

TalentPlus is a robust Full Stack solution designed for Human Resources administrative management and employee self-service information, implementing a secure, distributed architecture.

## Key Features
Intelligent Dashboard: Real-time administrative metrics (Employee Count, Status, etc.).

Employee Management: Complete CRUD operations for HR administrators.

Employee Self-Service API: Secure, token-protected endpoints (/me, /me/cv) allowing employees to query their own data and download their personalized PDF CV.

Public Registration: Unauthenticated endpoint for new employees to self-register.

Document Automation: Dynamic CV generation in PDF format using QuestPDF.

Export to Spreadsheet: Feature to export employee data to Excel or CSV format for external analysis.

Automated Testing: Inclusion of Unit and Integration Tests to validate core business logic and API responsiveness.

## Installation and Deployment (Docker)
The project is 100% dockerized. It requires Docker Desktop installed and running.

1. Step-by-Step Execution Guide
   Clone the Repository:

git clone https://github.com/THEMAIK17/TalentPlus.git
cd TalentPlus

2. Ensure Docker Desktop is active.

3. Execute the system: Open a terminal in the project root folder and run this command to build and start the services:

docker compose up --build

## Access Credentials (Data Seeder)
   The system uses a Data Seeder that automatically creates the database, the schema, and an administrator user to facilitate immediate review.
    Service,URL,User Type,Credentials

   Dashboard (Login),http://localhost:5200/Login,Administrator,User: admin@talentplus.com Password: Password123!
   Public API (Swagger),http://localhost:5200/swagger,Employee/Public,No login required

## Author
ARMANDO MAIKOL DURAN SALAS

Email: maikold774@gmail.com