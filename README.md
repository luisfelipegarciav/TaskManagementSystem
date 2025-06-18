# Task Management System

A modern, containerized ASP.NET Core web API for managing tasks, built with best practices in .NET, Docker, and DevOps.  
This project demonstrates a robust development and deployment workflow using environment variables, Docker Compose, and Nginx as a reverse proxy for production scenarios.

---

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
  - [Environment Variables (.env)](#environment-variables-env)
  - [Initial Admin User](#initial-admin-user)
  - [Docker Build & Push](#docker-build--push)
  - [Docker Compose Setup](#docker-compose-setup)
    - [Development](#development)
    - [Production](#production)
    - [Why Three Compose Files?](#why-three-compose-files)
    - [Nginx as Production Proxy](#nginx-as-production-proxy)
    - [Port Configuration](#port-configuration)
    - [Environment Variables & ASP.NET Core URLs](#environment-variables--aspnetcore-urls)
    - [Data Volumes & Removing Data](#data-volumes--removing-data)
    - [Migration Tools (Flyway)](#migration-tools-flyway)
  - [Project Usage](#project-usage)
    - [Starting in Detached Mode](#starting-in-detached-mode)
    - [Viewing Logs](#viewing-logs)
    - [Stopping and Restarting](#stopping-and-restarting)
- [Contributing](#contributing)
- [License](#license)
- [Additional Notes](#additional-notes)

---

## Features

- RESTful API for task management (CRUD operations)
- MariaDB database backend
- Entity Framework Core migrations via Flyway
- Dockerized for easy local development and production deployment
- Nginx reverse proxy for production
- Environment-based configuration using `.env`
- Health check endpoint (`/health`)
- Swagger UI (optionally exposed in production via env variable)

---

## Getting Started

### Environment Variables (.env)

You must provide a `.env` file at the project root with the following variables (example values shown):

```env
# MariaDB settings
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_DATABASE=taskmanagement
MYSQL_USER=tm_admin
MYSQL_PASSWORD=secure_password
DB_PORT=3306

# API settings
TskMgr_DatabaseProvider=mariadb
TskMgr_ConnectionStrings__MariaDbConnection="server=db;port=3306;database=taskmanagement;user=tm_admin;password=secure_password"
TskMgr_Jwt__Issuer=TaskManagementSystem.WebApi
TskMgr_Jwt__Audience=MyClient
TskMgr_Jwt__SecretKey=your_jwt_secret_key_here
API_PORT=80
API_Enable_Swagger=false
```

**Key points:**
- The API expects `API_Enable_Swagger` (not `ENABLE_SWAGGER`).  
  Set to `true` to enable Swagger UI, `false` for production.
- `TskMgr_ConnectionStrings__MariaDbConnection` should use the correct MariaDB credentials and reference the `db` host as defined in Docker Compose.
- `MYSQL_ROOT_PASSWORD`, `MYSQL_USER`, and `MYSQL_PASSWORD` are used by the MariaDB container.
- `TskMgr_Jwt__SecretKey` should be a strong, random value for JWT signing.

---

### Initial Admin User

On first run, the database will be initialized.  
**Use the following credentials to log in as the initial admin user:**

- **Username:** `admin`
- **Password:** `abc123456`  
  (Make sure to change this password after initial login for security.)

---

### Docker Build & Push

Build the API image from the root directory, specifying the Dockerfile location:

```sh
docker build -f src/TaskManagementSystem.WebApi/Dockerfile -t yourdockerhubuser/yourimagename:latest .
```

**Why specify the Dockerfile and build context?**
- `-f src/TaskManagementSystem.WebApi/Dockerfile` ensures the correct Dockerfile is used.
- `.` as context includes all project files needed for proper build.

Tag and push to Docker Hub:

```sh
# Optionally tag a version
docker tag yourdockerhubuser/yourimagename:latest yourdockerhubuser/yourimagename:1.1

# Push both tags
docker push yourdockerhubuser/yourimagename:latest
docker push yourdockerhubuser/yourimagename:1.1
```

---

### Docker Compose Setup

There are three Compose files:

- `docker-compose.yml` (base)
- `docker-compose.dev.yml`
- `docker-compose.prod.yml`

#### Development

Start all services in development mode:

```sh
docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

#### Production

Start all services in production mode:

```sh
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

#### Why Three Compose Files?

- **`docker-compose.yml`** contains shared configuration.
- **`docker-compose.dev.yml`** overrides for local/dev use (e.g. mounts, debug).
- **`docker-compose.prod.yml`** overrides for production (e.g. image tags, Nginx, health checks).

This approach allows flexible, environment-specific setups while avoiding duplication.

---

### Nginx as Production Proxy

- In production, Nginx acts as a reverse proxy to the API container.
- **Why?**  
  - Handles HTTPS (TLS termination), static file serving, and forwarding to the correct API port.
  - Improves security and scalability.
- Your API container listens on an internal port (e.g., 80), Nginx listens on external port (e.g., 5003 or 80/443).
- Nginx configuration is in `/etc/nginx/nginx.conf` (see repo for example).

---

### Port Configuration

- **Development:** API may listen on ports like 8080 (mapped to host).
- **Production:** API listens on 80 inside the container, Nginx proxies from 5003 (or 80/443).
- **Why is this important?**
  - Docker Compose networks services by name and internal port.
  - Nginx and Compose must agree on which port to use for internal traffic.

---

### Environment Variables & ASP.NET Core URLs

- `ASPNETCORE_URLS` (or `ASPNETCORE_HTTP_PORTS`) sets which port and address the API binds to.
- This is **crucial** in containers: if your API is not listening on the port Nginx or Compose expects, requests will fail.
- **Example:**  
  - `ASPNETCORE_URLS=http://+:80` means "listen on all interfaces, port 80".
  - If set to 8080, Nginx or Compose must forward to 8080.
- Use uppercase and underscores for all ENV vars for compatibility.

---

### Data Volumes & Removing Data

When running with Docker Compose, your MariaDB data is persisted in a Docker volume.  
- To stop the stack **without deleting data**:
  ```sh
  docker compose down
  ```
- To stop the stack **and delete all data volumes** (useful for a clean start, such as development resets or test environments):
  ```sh
  docker compose down --volumes
  ```
  **Warning:** `--volumes` will delete all persistent data, including your database! Use with caution.

---

### Migration Tools (Flyway)

This project uses a **migration-tools** service powered by [Flyway](https://hub.docker.com/r/flyway/flyway) to manage database migrations.  
- Migration scripts are located in the migrations folder and are automatically applied when the stack starts.
- **Script naming convention:**  
  Follow the Flyway standard:  
  ```
  V1__Initial.sql
  V2__AddUserTable.sql
  V3__AddTaskTable.sql
  ```
  - Start with `V`, followed by version, double underscore, then description.
  - Each new migration increments the version (`V2`, `V3`, etc.).
- For more details, see the [Flyway documentation](https://flywaydb.org/documentation/concepts/migrations).

---

## Project Usage

### Starting in Detached Mode

```sh
docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

### Viewing Logs

Show all logs:
```sh
docker compose logs -f
```

Show logs for a specific service (e.g., API):
```sh
docker compose logs -f api
```

### Stopping and Restarting

Stop all services (keeps database/data intact):
```sh
docker compose down
```

Stop **and remove all data volumes** (useful for a fresh start, will wipe your database!):
```sh
docker compose down --volumes
```

Restart with new env or code changes:
```sh
docker compose up -d --build
```

---

## Contributing

Pull requests and issues are welcome!  
Please open an issue first to discuss your proposal.

---

## License

This project is licensed under the MIT License.

---

## Additional Notes

- **Health Check:**  
  The `/health` endpoint is provided for container orchestration systems to check service status.

- **Database migrations:**  
  Handled automatically via the Flyway container during startup.

- **Service Discovery:**  
  Compose uses Docker networks; service names in Compose files act as DNS names (e.g., `api`, `db`).

- **Swagger UI in Production:**  
  Exposing Swagger in production should be controlled using the `API_Enable_Swagger` environment variable. For public deployments, restrict access to Swagger using authentication, IP allow-lists, or enable temporarily for debugging only.

- **.env and Secrets:**  
  Do **not** commit your `.env` file to version control if it contains sensitive data like passwords or JWT secrets. Use environment-specific secrets management in CI/CD pipelines.

- **Container Networking:**  
  All services (API, DB, migration tool, Nginx) are attached to a shared Docker Compose network for seamless name-based service discovery.

- **Production Best Practices:**  
  - Always use tagged images for production deployments (e.g., `:1.1`) rather than `:latest` to avoid unexpected changes.
  - Keep your Nginx, DB, and API containers updated with the latest security patches.

---

**If you have questions or need help, please open an issue or discussion on GitHub!**