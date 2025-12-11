# TalentoPlus - Sistema de Gestión de Talento Humano

Sistema de gestión de recursos humanos desarrollado con .NET 8, PostgreSQL y arquitectura limpia.

## � Repositorio
**GitHub**: [https://github.com/LDanielreyes/TalentoPlus](https://github.com/LDanielreyes/TalentoPlus)

---

## 🚀 Despliegue con Docker (RECOMENDADO)

### Requisitos
- Docker 20.10+
- Docker Compose v2+

### Pasos

1. **Clonar el repositorio**
```bash
git clone https://github.com/LDanielreyes/TalentoPlus.git
cd TalentoPlus
```

2. **Desplegar directamente** ✨
```bash
docker compose up -d --build
```

**¡Eso es todo!** El proyecto viene con valores por defecto funcionales. No necesitas configurar variables de entorno.

3. **Acceder a las aplicaciones**
- **Web Application**: http://localhost:5000
- **API REST**: http://localhost:5100
- **Swagger (Documentación)**: http://localhost:5100/swagger

> [!TIP]
> **Configuración Opcional**: Si necesitas cambiar la base de datos, credenciales SMTP o la API key de Gemini, puedes crear un archivo `.env` (ver sección Variables de Entorno). Los valores en `.env` sobrescribirán los valores por defecto.

### Comandos Útiles
```bash
# Ver estado
docker compose ps

# Ver logs
docker compose logs -f

# Detener
docker compose down

# Reiniciar
docker compose restart
```

---

## 💻 Ejecución Local (Sin Docker)

### Requisitos
- .NET 8 SDK
- PostgreSQL 15+

### Pasos

1. **Configurar base de datos**
```sql
CREATE DATABASE talentodb;
```

2. **Configurar `appsettings.json`**

Editar `TalentoPlusWeb/appsettings.json` y `TalentoPlusAPI/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=talentodb;Username=postgres;Password=TU_PASSWORD"
  }
}
```

3. **Aplicar migraciones**
```bash
dotnet ef database update --project TalentoPlus.Infrastructure --startup-project TalentoPlusWeb
```

4. **Ejecutar aplicaciones**

**Web (Terminal 1):**
```bash
cd TalentoPlusWeb
dotnet run
```
Acceso: https://localhost:5001

**API (Terminal 2 - Opcional):**
```bash
cd TalentoPlusAPI
dotnet run
```
Acceso: https://localhost:5101/swagger

---

## ⚙️ Variables de Entorno (OPCIONAL)

El proyecto **funciona sin configuración** usando valores por defecto incluidos en `docker-compose.yml`.

### Sobrescribir Valores (Opcional)

Si necesitas usar tu propia base de datos, configuración SMTP o API key de Gemini, crea un archivo `.env`:

1. **Copiar plantilla**:
```bash
cp .env.example .env
```

2. **Editar con tus valores**:

```bash
# Database (solo si quieres usar otra base de datos)
DATABASE_CONNECTION_STRING=Host=xxx.clever-cloud.com;Port=xxx;Database=xxx;Username=xxx;Password=xxx;SSL Mode=Require;Trust Server Certificate=true

# JWT (solo si quieres cambiar las claves por defecto)
JWT_SECRET=tu_clave_secreta_minimo_32_caracteres
JWT_ISSUER=TalentoPlusAPI
JWT_AUDIENCE=TalentoPlusClients

# SMTP (solo si quieres enviar emails reales)
SMTP_HOST=smtp.ejemplo.com
SMTP_PORT=465
SMTP_USER=tu_email@ejemplo.com
SMTP_PASS=tu_password
SMTP_ENABLE_SSL=true

# Gemini AI (solo si quieres usar funciones de IA)
GEMINI_API_KEY=tu_api_key
```

> [!NOTE]
> **Valores por defecto incluidos:**
> - **Base de datos**: PostgreSQL en Clever Cloud (ya configurada)
> - **JWT**: Claves seguras pre-configuradas
> - **SMTP**: Configuración de ejemplo (emails no se enviarán sin credenciales reales)
> - **Gemini AI**: Vacío (funcionalidades de IA deshabilitadas por defecto)

---

## 🔐 Credenciales de Acceso

### Usuario Administrador
```
Email: admin@talentoplus.com
Password: Admin123!
```

### Trabajadores Importados
Los trabajadores importados desde Excel tienen:
```
Password: Worker@123
```
⚠️ **Debe cambiarse en el primer login**

---

## 🧪 Pruebas

Ejecutar todas las pruebas:
```bash
dotnet test
```

**Cobertura**: 5 pruebas (1 unitaria + 4 integración)

---

## 📁 Estructura del Proyecto

```
TalentoPlus/
├── TalentoPlus.Domain/          # Entidades y modelos
├── TalentoPlus.Application/     # Lógica de negocio
├── TalentoPlus.Infrastructure/  # Acceso a datos
├── TalentoPlusWeb/              # Aplicación MVC
├── TalentoPlusAPI/              # API RESTful
├── TalentoPlus.Test/            # Pruebas
├── docker-compose.yml           # Orquestación Docker
└── README.md
```

---

## 📊 Funcionalidades Principales

- ✅ Gestión completa de trabajadores (CRUD)
- ✅ Importación masiva desde Excel
- ✅ Autenticación con Identity y JWT
- ✅ Roles (Admin, Worker)
- ✅ API RESTful documentada (Swagger)
- ✅ Integración con PostgreSQL

---

## 📖 API Endpoints

Documentación completa en: http://localhost:5100/swagger

**Principales endpoints**:
- `GET /api/workers` - Listar trabajadores
- `POST /api/workers` - Crear trabajador
- `PUT /api/workers/{id}` - Actualizar
- `DELETE /api/workers/{id}` - Eliminar
- `POST /api/auth/login` - Iniciar sesión

---

## �️ Tecnologías

- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- PostgreSQL (Clever Cloud)
- Entity Framework Core
- Identity & JWT
- Docker & Docker Compose
- xUnit

---

## � Autor

**Lucas Chacón**  
Email: lucasdanielchaconr@gmail.com

---

## 📝 Notas

- La base de datos está alojada en **Clever Cloud** (ya configurada por defecto)
- El proyecto funciona **sin configuración** - despliega directamente con `docker compose up`
- El archivo `.env` es **opcional** - solo necesario si quieres sobrescribir valores por defecto
- Use `.env.example` como plantilla si necesita configuración personalizada
- Nunca commita el archivo `.env` al repositorio
- Consulte `DOCKER_README.md` para detalles adicionales de Docker

