# Configuración local

Sigue estos pasos en tu equipo. Los valores reales se guardan en user-secrets y no se copian al repositorio.

## 1. Instala MySQL

Instala la misma versión de MySQL que usa el equipo (por ejemplo, la versión utilizada en lab11) e inicia el servidor local. Crea o confirma que puedes usar la base `skyport_db`.

## 2. Configura los secretos del proyecto Web

Desde la raíz del repositorio, inicializa user-secrets si el proyecto todavía no tiene un `UserSecretsId`:

```powershell
dotnet user-secrets init --project src/SkyPort.Web
```

Guarda la cadena usando tu usuario y tu propia clave local de MySQL:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=skyport_db;User=root;Password=TU_CLAVE_LOCAL;" --project src/SkyPort.Web
```

Configura el correo y la contraseña que usará el administrador inicial en tu entorno local:

```powershell
dotnet user-secrets set "AdminSeed:Email" "tu-correo-local@example.com" --project src/SkyPort.Web
dotnet user-secrets set "AdminSeed:Password" "TU_CLAVE_LOCAL_DEL_ADMIN" --project src/SkyPort.Web
```

No guardes esos valores en `appsettings.json`, en un commit ni en un mensaje del equipo. `appsettings.example.json` solo contiene marcadores `CAMBIAR` para mostrar el formato.

## 3. Crea y prepara la base de datos

Ejecuta los scripts desde un cliente MySQL en este orden:

1. `database/01_schema.sql`
2. `database/02_identity.sql`
3. `database/03_seed.sql`

Los tres scripts ya están disponibles. El segundo crea las siete tablas estándar de ASP.NET Core Identity con MySQL, sin migraciones; consulta [IDENTITY.md](IDENTITY.md) para su generación y validación. El tercero carga datos ficticios repetibles; las cantidades y comprobaciones están en [DATOS_PRUEBA.md](DATOS_PRUEBA.md). Antes de S07, cada integrante debe ejecutar los tres archivos y comprobar que existen las 14 tablas operativas y las siete de Identity en su propia base `skyport_db`, con su cadena local configurada.

## 4. Restaura las herramientas y compila

Restaura la herramienta local fijada por el repositorio y compila la solución:

```powershell
dotnet tool restore
dotnet restore SkyPort.sln
dotnet build SkyPort.sln
```

El manifiesto fija `dotnet-ef` en la versión 9.0.0. Si `dotnet ef` indica que falta el runtime de .NET 9, instala el SDK/runtime de .NET 9 o permite el roll-forward de .NET configurando `DOTNET_ROLL_FORWARD=Major` en la sesión de PowerShell:

```powershell
$env:DOTNET_ROLL_FORWARD = "Major"
```
