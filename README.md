# Technical Backend Test

API REST desarrollada en .NET 10 con Minimal API, Entity Framework Core,
SQLite, FluentValidation, CQRS por carpetas, Swagger UI y seguridad por API Key.

## Version de .NET

- SDK probado: .NET SDK 10.0.301
- Target framework del proyecto: `net10.0`

## Como correr el proyecto

Desde la raiz del repositorio:

```bash
dotnet restore TechnicalBackendTest/TechnicalBackendTest.Api/TechnicalBackendTest.Api.csproj
dotnet build TechnicalBackendTest/TechnicalBackendTest.Api/TechnicalBackendTest.Api.csproj
dotnet run --project TechnicalBackendTest/TechnicalBackendTest.Api/TechnicalBackendTest.Api.csproj
```

El perfil local expone la API en:

- `http://localhost:5229`
- `https://localhost:7024`

En ambiente Development, Swagger UI queda disponible en:

- `http://localhost:5229/swagger`

## Base de datos SQLite

La cadena de conexion esta configurada en
`TechnicalBackendTest/TechnicalBackendTest.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=technical_backend.db"
  }
}
```

La base local se crea/aplica con migraciones de EF Core:

```bash
dotnet ef database update --project TechnicalBackendTest/TechnicalBackendTest.Api/TechnicalBackendTest.Api.csproj
```

## Seguridad por API Key

Toda la API funcional requiere el header:

```http
X-API-KEY: local-dev-api-key
```

La API Key de prueba esta configurada en `appsettings.json`:

```json
{
  "Security": {
    "ApiKey": "local-dev-api-key"
  }
}
```

Si el header no se envia o el valor es incorrecto, la API responde `401 Unauthorized`.

## Endpoints implementados

### Users

- `POST /users`
- `POST /users/check-password`
- `GET /users`
- `GET /users?isActive=true|false`
- `GET /users/{id}`
- `PUT /users/{id}`
- `DELETE /users/{id}`

### Addresses

- `POST /users/{userId}/addresses`
- `GET /users/{userId}/addresses`
- `PUT /addresses/{id}`
- `DELETE /addresses/{id}`

### Currencies

- `GET /currencies`
- `POST /currencies`
- `POST /currency/convert`

## Password de usuarios

El body requerido para crear usuarios sigue el enunciado de la prueba:

```json
{
  "name": "Juan",
  "email": "juan@test.com"
}
```

Al crear un usuario, la API usa el valor de `name` como password inicial y lo
guarda hasheado en `PasswordHash` usando `PasswordHasher<User>`. El hash no se
expone en las respuestas.

Para verificar una password se puede usar:

```bash
curl -X POST http://localhost:5229/users/check-password \
  -H "Content-Type: application/json" \
  -H "X-API-KEY: local-dev-api-key" \
  -d '{"id":1,"password":"Juan"}'
```

Respuesta esperada:

```json
{
  "isCorrect": true
}
```

## Ejemplos rapidos

Crear usuario:

```bash
curl -X POST http://localhost:5229/users \
  -H "Content-Type: application/json" \
  -H "X-API-KEY: local-dev-api-key" \
  -d '{"name":"Juan","email":"juan@test.com"}'
```

Verificar password de usuario:

```bash
curl -X POST http://localhost:5229/users/check-password \
  -H "Content-Type: application/json" \
  -H "X-API-KEY: local-dev-api-key" \
  -d '{"id":1,"password":"Juan"}'
```

Crear moneda:

```bash
curl -X POST http://localhost:5229/currencies \
  -H "Content-Type: application/json" \
  -H "X-API-KEY: local-dev-api-key" \
  -d '{"code":"USD","name":"United States Dollar","rateToBase":7300}'
```

Convertir divisas:

```bash
curl -X POST http://localhost:5229/currency/convert \
  -H "Content-Type: application/json" \
  -H "X-API-KEY: local-dev-api-key" \
  -d '{"fromCurrencyCode":"USD","toCurrencyCode":"PYG","amount":100}'
```

Tambien dentro del repositorio se encuentra una carpeta llamada "http" la cual
sirve para probar todos los casos que se tuvieron en cuenta al momento de realizar la prueba, en la coleccion de postman se puede ejecutar la coleccion en orden para probar el flujo completo directamente.

## Estado de implementacion

Implementado:

- Minimal API en .NET 10.
- CRUD de usuarios.
- Password de usuarios guardada como hash.
- Endpoint para verificar password de usuario.
- CRUD de direcciones relacionadas a usuarios.
- Tabla de monedas y modulo de conversion de divisas.
- EF Core con SQLite y migracion inicial.
- Validaciones con FluentValidation.
- Separacion CQRS en carpetas de Application.
- Seguridad por `X-API-KEY`.
- Swagger UI en Development con esquema de API Key.

Checklist contra la prueba tecnica:

- Users: crear, listar, filtrar por `isActive`, obtener por id, modificar y eliminar.
- Addresses: crear/listar por usuario, modificar y eliminar.
- Currency: listar/crear monedas y convertir divisas con `RateToBase`.
- Seguridad: toda la API pasa por `X-API-KEY` y responde `401` si falta o es invalida.
- Persistencia: EF Core con SQLite, `DbContext` y migracion inicial.
- Validacion: FluentValidation en requests de usuarios, direcciones, monedas y conversion.
- Organizacion: comandos/queries separados por carpetas de `Application`.
- Swagger: disponible en Development.

No quedan puntos grandes pendientes respecto al alcance solicitado. Como mejora
futura, se podria agregar un contrato explicito para cambiar password en lugar
de depender del `name` inicial.
