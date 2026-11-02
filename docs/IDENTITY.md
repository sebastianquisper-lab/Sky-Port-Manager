# J03: tablas de ASP.NET Core Identity sin migraciones

`database/02_identity.sql` instala las siete tablas estándar de Identity en la base MySQL `skyport_db` previamente creada con J02:

- `AspNetRoles`
- `AspNetUsers`
- `AspNetRoleClaims`
- `AspNetUserClaims`
- `AspNetUserLogins`
- `AspNetUserRoles`
- `AspNetUserTokens`

El DDL se generó desde `SkyPortIdentityDbContext`, en `src/SkyPort.Persistence/Context`, usando `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 9.0.0 y `Pomelo.EntityFrameworkCore.MySql` 9.0.0. No se utilizó el proveedor de SQL Server ni comandos de migraciones. El contexto usa el modelo estándar de Identity; este cambio no registra todavía los servicios de autenticación en la aplicación.

## Generación

La fábrica de diseño `src/SkyPort.Web/DesignTime/SkyPortIdentityDbContextFactory.cs` permite ejecutar EF sin modificar `Program.cs`. Lee `ConnectionStrings:DefaultConnection` de los user-secrets de Web o de `ConnectionStrings__DefaultConnection` en el entorno (el entorno tiene prioridad). No contiene cadenas ni contraseñas guardadas en el código.

La conexión debe apuntar a `skyport_db`. La fábrica abre esa base para confirmar su existencia y usa `ServerVersion.AutoDetect` para consultar la versión real del servidor. Rechaza MariaDB y MySQL anterior a 8.0.16; no crea la base si falta.

Después de configurar los secretos como indica [CONFIGURACION.md](CONFIGURACION.md), desde la raíz del repositorio:

```powershell
dotnet tool restore
dotnet ef dbcontext script --context SkyPortIdentityDbContext --project src/SkyPort.Persistence --startup-project src/SkyPort.Web --output database/02_identity.sql
```

Este comando **sobrescribe el archivo**, por lo que después se debe volver a envolver todo el DDL generado como muestra el script versionado: `USE skyport_db`, eliminación de un procedimiento instalador residual, `DELIMITER //`, `CREATE PROCEDURE sp_instalar_identity()`, condición de ausencia de `AspNetUsers`, DDL completo, cierre del procedimiento, `DELIMITER ;`, `CALL` y `DROP PROCEDURE`. También se conserva `COLLATE utf8mb4_unicode_ci` en el `ALTER DATABASE` generado para mantener la collation de J02. Las definiciones de tablas e índices son las generadas por Pomelo.

La fábrica de diseño es un mecanismo de EF documentado por [Microsoft](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation). La detección de versión corresponde a la [configuración de Pomelo](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options).

## Ejecución y repetibilidad

En MySQL Workbench, abrir y ejecutar completo `database/02_identity.sql` después de `01_schema.sql`. En CMD también se puede usar:

```bat
mysql -u root -p < database/01_schema.sql
mysql -u root -p < database/02_identity.sql
mysql -u root -p < database/02_identity.sql
```

En PowerShell:

```powershell
Get-Content -Raw database/01_schema.sql | mysql -u root -p
Get-Content -Raw database/02_identity.sql | mysql -u root -p
Get-Content -Raw database/02_identity.sql | mysql -u root -p
```

El usuario de instalación necesita permisos para crear tablas, índices y procedimientos, alterar la base y ejecutar y eliminar el procedimiento. `DELIMITER` es una directiva del cliente MySQL; hay que ejecutar el archivo completo con un cliente que la reconozca.

Cuando `AspNetUsers` ya existe, se omite todo el DDL, incluidos los `CREATE INDEX`. El procedimiento se elimina al finalizar. La instalación no repara tablas faltantes ni estructuras incompatibles si ya existe `AspNetUsers`; tampoco es una migración de versiones del modelo. El primer `DROP PROCEDURE IF EXISTS` permite repetir el archivo tras una ejecución interrumpida, pero no repara una instalación parcial.

## Validación de J03

Ejecutado el 2 de noviembre de 2026 contra MySQL Community Server 8.4.13 en la instancia temporal local de J02, que ya tenía sus 14 tablas:

- `dotnet ef dbcontext script` completado con Pomelo 9.0.0 y detección de la versión del servidor existente.
- Dos ejecuciones consecutivas de `02_identity.sql`, ambas con código de salida 0.
- 21 tablas en total: las 14 de J02 y las siete de Identity, todas InnoDB.
- Ningún procedimiento `sp_instalar_identity` residual.
- Ninguna tabla `__EFMigrationsHistory` ni carpeta `Migrations` creada.
- La base conserva utf8mb4 y utf8mb4_unicode_ci.
- `dotnet build SkyPort.sln --no-restore`: sin errores ni advertencias.
- `dotnet test SkyPort.sln --no-build --no-restore`: ocho pruebas existentes aprobadas.

Para revisar estos resultados en una base que solo contiene J02 y J03:

```sql
SELECT COUNT(*) AS TablasTotales
FROM information_schema.tables
WHERE table_schema = 'skyport_db';

SELECT table_name, engine
FROM information_schema.tables
WHERE table_schema = 'skyport_db' AND table_name LIKE 'AspNet%';

SELECT COUNT(*) AS ProcedimientosResiduales
FROM information_schema.routines
WHERE routine_schema = 'skyport_db' AND routine_name = 'sp_instalar_identity';

SELECT COUNT(*) AS TablasMigraciones
FROM information_schema.tables
WHERE table_schema = 'skyport_db' AND table_name = '__EFMigrationsHistory';
```
