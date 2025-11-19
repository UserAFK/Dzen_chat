# DzenChat: Real-Time Communication Platform

DzenChat is a modern, full-stack application designed for real-time communication. The project is built using a microservices-like architecture, featuring an ASP.NET Core backend, an Angular frontend, and a Microsoft SQL Server database, orchestrated via Docker Compose for local development and deployed to Google Cloud Run for production.

# Technologies Stack

|Component | Technology | Description |
| :--- | :--- | :--- |
|Backend | ASP.NET Core | Handles business logic, API endpoints, and reCAPTCHA verification. |
|Frontend | Angular | User interface and client-side logic. |
|Database | MS SQL Server | Persistent data storage. |
|Container | Docker / Docker | Compose	Manages local development and service networking. |
|Deployment | Google Cloud Run | Serverless platform for production deployment. |

# Local Development Setup

The easiest way to run DzenChat locally is using Docker Compose, which sets up the database, backend API, and frontend web service in a single environment.

## Prerequisites

Docker and Docker Compose installed.

1. A .env file in the root directory for database secrets.

Create a file named .env in the root directory of the project and add your MSSQL Server password:

```SA_PASSWORD=YourStrongLocalPassword123!```

2. Build and Run. Run the following commands from the project root:

Build the images (Backend and Frontend)
```bash
docker-compose build
```

Start all services in detached mode
```bash
docker-compose up -d
```

## Access

| Component | URL (Local Host) | Port |
| :--- | :--- | :--- |
| Frontend Web | http://localhost:4200 | Access the application here. |
| Backend API | http://localhost:7242 | For direct API calls or testing. |
| MSSQL Server | localhost:1433 | Database access. |

# Deployment to Google Cloud Run

The project uses GitHub Actions for continuous deployment to Google Cloud Run. The workflow executes on push.
https://frontend-service-656951842437.europe-north1.run.app/