
# Task Manager Application Docker Setup

## Overview
This document provides a detailed guide to set up and run the **Task Manager Application** using Docker containers. The application includes:

- A **.NET API** running in a Docker container.
- A **SQL Server** instance running in another Docker container.
- Communication between the API and database using Docker networking.

---

## Prerequisites

1. **Docker Installed**:
   - Download and install Docker from [docker.com](https://www.docker.com/).

2. **Task Manager Application Source Code**:
   - Ensure the application directory contains:
     ```
     /TaskManager/
     ├── Dockerfile
     ├── TaskManager.csproj
     ├── Program.cs
     ├── Startup.cs
     ├── ...
     ```

3. **SQL Server Credentials**:
   - Username: `sa`
   - Password: `YourSecurePassword`

---

## Step-by-Step Setup

### 1. Create and Configure the Dockerfile
Place the following `Dockerfile` in the root of the `TaskManager` directory:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskManager.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.dll"]
```

### 2. Build the Docker Image for the API

Run the following commands to build the Docker image for the API:

```bash
cd /path/to/TaskManager

docker build -t taskmanager-api .
```

### 3. Set Up the SQL Server Container

Run a Docker container for SQL Server:

```bash
docker run --name sqlserver \
    -e "ACCEPT_EULA=Y" \
    -e "SA_PASSWORD=YourSecurePassword" \
    -p 1433:1433 \
    -d mcr.microsoft.com/mssql/server:2019-latest
```

### 4. Create a Custom Docker Network

To ensure communication between the API and SQL Server containers, create a custom Docker network:

```bash
docker network create app-network
```

### 5. Connect Containers to the Network

1. Connect the SQL Server container to the network:
   ```bash
   docker network connect app-network sqlserver
   ```

2. Run the API container and connect it to the same network:
   ```bash
   docker run --name taskmanager-api \
       -e "ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=TaskManagerDB;User=sa;Password=YourSecurePassword;" \
       --network app-network \
       -p 5001:80 \
       -d taskmanager-api
   ```

### 6. Verify Connectivity

#### 6.1 Test API Endpoints
Access the API in your browser or Postman:
```
http://localhost:5000/api/tasks
```

#### 6.2 Test Database Connectivity
1. Open a shell inside the API container:
   ```bash
   docker exec -it taskmanager-api /bin/bash
   ```
2. Ping the SQL Server container:
   ```bash
   ping sqlserver
   ```

3. Verify SQL Server is reachable on port 1433.

### 7. Troubleshooting

#### Common Issues
1. **Connection Reset Errors**:
   - Ensure the containers are on the same Docker network.
   - Verify the SQL Server container is running (`docker ps`).

2. **SQL Server Authentication Issues**:
   - Ensure the `SA_PASSWORD` meets SQL Server’s complexity requirements.
   - Verify the `TaskManagerDB` database exists.

3. **API Binding Issues**:
   - Update `Program.cs` to bind the API to `0.0.0.0`:
     ```csharp
     builder.WebHost.ConfigureKestrel(options =>
     {
         options.ListenAnyIP(80);
     });
     ```

### 8. Logs and Debugging

#### View Container Logs
- API Container:
  ```bash
  docker logs taskmanager-api
  ```

- SQL Server Container:
  ```bash
  docker logs sqlserver
  ```

---

## Next Steps
- Add proper HTTPS support for production.
- Use `docker-compose` for simplified container orchestration.
- Implement monitoring and logging for containerized services.

---

## Contact
For further assistance, contact the development team or refer to the Docker and .NET documentation.

