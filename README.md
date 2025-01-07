
# Task Manager Application Docker Setup

## Overview
This document provides a detailed guide to set up and run the **Task Manager Application** using Docker containers. The application includes:

- > [!NOTE]
> I am working on configuting k8s to simplify this, but these are the current steps to reproduce.

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
3. **SQL Server Container**
    ```bash
    - docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourdatabasepassword' \
    - -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
    ```
    
4. **SQL Server Credentials**:
   - Username: `sa`
   - Password: `YourSecurePassword`

---

## Step-by-Step Setup


### 1. Build the Docker Image for the API

Run the following commands to build the Docker image for the API:

```bash
cd /path/to/TaskManager

docker build -t taskmanager-api .
```

### 2. Set Up the SQL Server Container

Run a Docker container for SQL Server:

```bash
docker run --name sqlserver \
    -e "ACCEPT_EULA=Y" \
    -e "SA_PASSWORD=YourSecurePassword" \
    -p 1433:1433 \
    -d mcr.microsoft.com/mssql/server:2019-latest
```

### 3. Create a Custom Docker Network

To ensure communication between the API and SQL Server containers, create a custom Docker network:

```bash
docker network create app-network
```

### 4. Connect Containers to the Network

1. Connect the SQL Server container to the network:
   ```bash
   docker network connect app-network sqlserver
   ```

2. Run the API container and connect it to the same network:
    - > [!NOTE]
    > I haven't setup DNS to resolve sqlserver for my docker network yet.
    > You can run "docker inspect network app-network" or "docker inspect sqlserver" to get the ip address instead. 

   ```bash
   docker run --name taskmanager-api \
       -e DefaultConnection="Server=sqlserver,1433;Database=TaskManagerDB;User=sa;Password=YourSecurePassword;" \
       --network app-network \
       -p 5001:80 \
       -d taskmanager-api
   ```

### 5. Verify Connectivity

#### 5.1 Test API Endpoints
Access the API in your browser or Postman:
```
http://localhost:5000/api/tasks
```

#### 5.2 Test Database Connectivity
1. Open a shell inside the API container:
   ```bash
   docker exec -it taskmanager-api /bin/bash
   ```
2. Ping the SQL Server container:
   ```bash
   ping sqlserver
   ```

3. Verify SQL Server is reachable on port 1433.

### 6. Populate the database

```
docker ef update database
```

> [!NOTE]
> Currently the migration doesn't populate the database with categories,
> you will need to add some to the database through sqlcmd, sqlserver, or Azure Data Studio

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
- Create Kubernetes yaml files for starting up the containers.
- Update EF Core migrations to include default category values
- Create Volumes and shell script to update the database outside of the standard build
- Code the rest of the basic endpoints
- Implement monitoring and logging for containerized services.

---


