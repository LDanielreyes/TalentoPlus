# TalentoPlus - Sistema de Gestión de Talento Humano

Sistema de gestión de recursos humanos desarrollado con .NET 8, PostgreSQL y arquitectura limpia.

## � Repositorio
**GitHub**: [https://github.com/TU_USUARIO/TalentoPlus](https://github.com/TU_USUARIO/TalentoPlus)

---

## 🚀 Despliegue con Docker (RECOMENDADO)

### Requisitos
- Docker 20.10+
- Docker Compose v2+

### Pasos

1. **Clonar el repositorio**
```bash
git clone https://github.com/TU_USUARIO/TalentoPlus.git
cd TalentoPlus
```

2. **Configurar variables de entorno**

Copiar `.env.example` a `.env`:
```bash
cp .env.example .env
```

Editar `.env` con tus valores (ver sección Variables de Entorno).

3. **Desplegar**
```bash
docker compose up -d --build
```

4. **Acceder a las aplicaciones**
- **Web Application**: http://localhost:5000
- **API REST**: http://localhost:5100
- **Swagger (Documentación)**: http://localhost:5100/swagger

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

## ⚙️ Variables de Entorno

### Archivo .env
El archivo `.env` debe contener:

```bash
# Database (Clever Cloud)
DATABASE_CONNECTION_STRING=Host=xxx.clever-cloud.com;Port=xxx;Database=xxx;Username=xxx;Password=xxx;SSL Mode=Require;Trust Server Certificate=true

# JWT
JWT_SECRET=tu_clave_secreta_minimo_32_caracteres
JWT_ISSUER=TalentoPlusAPI
JWT_AUDIENCE=TalentoPlusClients

# SMTP (Email)
SMTP_HOST=smtp.ejemplo.com
SMTP_PORT=465
SMTP_USER=tu_email@ejemplo.com
SMTP_PASS=tu_password
SMTP_ENABLE_SSL=true

# Gemini AI (Opcional)
GEMINI_API_KEY=tu_api_key
```

**Nota**: El archivo `.env.example` contiene una plantilla completa.

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
Email: lucas.chaconc@pca.edu.co

---

## 📝 Notas

- La base de datos está alojada en **Clever Cloud**
- Use `.env.example` como plantilla para configuración
- Nunca commita el archivo `.env` al repositorio
- Consulte `DOCKER_README.md` para detalles adicionales de Docker
