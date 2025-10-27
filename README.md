# Gestor-de-Tareas-Colaborativo

Aplicación ASP.NET Core 8 con Razor Pages para gestionar tareas con asignación de usuarios, estados y UI responsive.

## Tecnologías
- ASP.NET Core 8 + Razor Pages
- Entity Framework Core + PostgreSQL (Npgsql)
- HTML/CSS (Flex/Grid), JavaScript (fetch para cambios de estado)

## Ejecutar
1. Configura la cadena de conexión en `TaskManager/appsettings.json` (PostgreSQL).
2. Aplica migraciones (si corresponde): `dotnet ef database update`
3. Ejecuta: `dotnet run` en `TaskManager/TaskManager/`

## Estructura relevante
- `TaskManager/Pages/` UI (Razor): `Index.cshtml` + `Index.cshtml.cs`
- `TaskManager/Core/` Dominio: `Tarea.cs`, `Enums/EstadoTarea.cs`
- `TaskManager/Infrastructure/ApplicationDbContext.cs` EF Core
- `TaskManager/wwwroot/` Estáticos: `css/site.css`, `js/site.js`
- `TaskManager/Program.cs` DI, RazorPages, DbContext

## Notas
- `Titulo` es requerido; se valida con `ModelState`.
- Cambio de estado sin recargar usando `fetch` hacia `?handler=UpdateEstado`.
- Tras crear una tarea, se muestra un modal de confirmación.

## Configuración de entorno (.env / User Secrets)
- El proyecto carga variables desde `.env` (si existe) usando `DotNetEnv` y desde User Secrets en Development.
- Orden de carga: `appsettings.json` → `appsettings.{Environment}.json` → User Secrets (Dev) → Variables de entorno → `.env` → línea de comandos (la más específica sobrescribe).

### Opción A: .env (local, gitignored)
1. Copia `TaskManager/TaskManager/.env.example` a `TaskManager/TaskManager/.env`.
2. Edita `.env` y coloca tu cadena real, por ejemplo:
   ```env
   ConnectionStrings__DefaultConnection="Host=localhost;Database=gestor_tareas_db;Username=postgres;Password=YOUR_PASSWORD"
   ```

### Opción B: User Secrets (recomendada para Development)
En `TaskManager/TaskManager/`:
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=gestor_tareas_db;Username=postgres;Password=YOUR_PASSWORD"
```
