# TalentoPlus - Docker Deployment Guide

## Prerequisites
- Docker installed
- Docker Compose installed

## Quick Start

### 1. Configure Environment Variables

Copy the `.env.example` file to `.env`:

```bash
cp .env.example .env
```

Edit `.env` and fill in your values:
- `DB_PASSWORD`: PostgreSQL password
- `JWT_SECRET`: JWT secret key (minimum 32 characters)
- `SMTP_USER`: SMTP email
- `SMTP_PASS`: SMTP password
- `GEMINI_API_KEY`: Gemini AI API key

### 2. Build and Run

```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

### 3. Access the Application

- **Web Application**: http://localhost:5000
- **API**: http://localhost:5100
- **API Documentation (Swagger)**: http://localhost:5100/swagger

### 4. Database Migrations

The database will be created automatically. Run migrations:

```bash
# From inside the API container
docker-compose exec api dotnet ef database update
```

## Services

- **postgres**: PostgreSQL 15 database
- **api**: TalentoPlus REST API (.NET 8)
- **web**: TalentoPlus MVC Web Application (.NET 8)

## Stopping the Application

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes all data)
docker-compose down -v
```

## Logs

```bash
# View all logs
docker-compose logs

# View specific service logs
docker-compose logs api
docker-compose logs web
docker-compose logs postgres

# Follow logs in real-time
docker-compose logs -f
```

## Troubleshooting

### Port Already in Use
If ports 5000, 5100, or 5432 are already in use, edit `docker-compose.yml` to change the port mappings.

### Database Connection Issues
Ensure the postgres service is healthy:
```bash
docker-compose ps
```

### Reset Everything
```bash
docker-compose down -v
docker-compose up --build
```
