# Base de datos de SkyPort Manager

## Requisitos y orden de ejecución

Usar MySQL 8.0.16 o superior, con InnoDB y utf8mb4. Los scripts de esta carpeta usan MySQL, conforme a J02–J05; no se ejecutan con el proveedor de SQL Server. La cadena local de la aplicación se configura en user-secrets como indica [CONFIGURACION.md](../docs/CONFIGURACION.md).

Ejecutar desde una conexión al servidor local, en este orden y deteniéndose si un archivo informa errores:

1. [01_schema.sql](01_schema.sql): crea `skyport_db` y las 14 tablas operativas.
2. [02_identity.sql](02_identity.sql): crea las siete tablas estándar de ASP.NET Core Identity, sin migraciones.
3. [03_seed.sql](03_seed.sql): carga los datos ficticios de prueba.

Los tres archivos admiten repetición. `01_schema.sql` no actualiza tablas existentes; `02_identity.sql` omite su instalación si ya existe `AspNetUsers`; `03_seed.sql` agrega solo las filas ausentes, sin reprogramar las fechas de vuelos ya insertados. Si cambió la estructura, seguir el procedimiento de reinicio de esta guía.

## Ejecutar desde Rider: herramienta Database

1. Abrir **View → Tool Windows → Database**.
2. Agregar una fuente de datos **MySQL** con el botón **+ → Data Source → MySQL**. Indicar el host local, puerto, usuario y contraseña propios. Si la base aún no existe, conectar al servidor sin seleccionar `skyport_db` (se puede usar la base del sistema `mysql`).
3. Descargar el controlador JDBC si Rider lo solicita, pulsar **Test Connection** y guardar la conexión.
4. En la herramienta Database, hacer clic derecho en la fuente de datos y elegir **SQL Scripts → Run SQL Script…**. Seleccionar `database/01_schema.sql` y ejecutarlo completo.
5. Refrescar la fuente de datos y seleccionar `skyport_db` entre los esquemas visibles. Ejecutar del mismo modo `02_identity.sql` y luego `03_seed.sql`, comprobando el resultado de cada archivo.
6. Abrir una consola SQL de esa fuente de datos y ejecutar la consulta de conteos incluida más abajo.

Ejecutar completo `02_identity.sql`, conservando sus directivas `DELIMITER` y su procedimiento instalador. Las opciones de creación de la conexión y ejecución de archivos se describen en la documentación de JetBrains: [MySQL en Rider](https://www.jetbrains.com/help/rider/MySQL.html) y [Run SQL files](https://www.jetbrains.com/help/rider/Run_SQL_Files.html).

## Ejecutar con el cliente mysql

Desde la raíz del repositorio, en CMD de Windows o una terminal compatible con la redirección `<`:

```bat
mysql -u root -p < database/01_schema.sql
mysql -u root -p < database/02_identity.sql
mysql -u root -p < database/03_seed.sql
```

La contraseña se introduce cuando el cliente la solicita; no se guarda en los comandos ni en el repositorio. Se puede cambiar `root` por el usuario local autorizado y agregar `--host=127.0.0.1 --port=3306` si corresponde. El ejecutable `mysql` debe estar en PATH o invocarse mediante su ruta completa.

PowerShell no admite esa redirección de entrada. En la terminal PowerShell de Rider, usar:

```powershell
Get-Content -Raw database/01_schema.sql | mysql -u root -p
Get-Content -Raw database/02_identity.sql | mysql -u root -p
Get-Content -Raw database/03_seed.sql | mysql -u root -p
```

Comprobar que cada ejecución termine sin errores antes de continuar.

## Verificar las tablas y los conteos

Después de una carga nueva, deben existir **21 tablas**: 14 operativas y siete de Identity. Ejecutar en la consola de Rider o en el cliente `mysql -u root -p`:

```sql
USE skyport_db;
SHOW TABLES;

SELECT 'Aerolinea' AS Tabla, COUNT(*) AS Filas, 4 AS Esperadas FROM Aerolinea
UNION ALL SELECT 'Aeronave', COUNT(*), 8 FROM Aeronave
UNION ALL SELECT 'Terminal', COUNT(*), 2 FROM Terminal
UNION ALL SELECT 'Puerta', COUNT(*), 10 FROM Puerta
UNION ALL SELECT 'Ruta', COUNT(*), 10 FROM Ruta
UNION ALL SELECT 'Vuelo', COUNT(*), 12 FROM Vuelo
UNION ALL SELECT 'Pasajero', COUNT(*), 20 FROM Pasajero
UNION ALL SELECT 'Reserva', COUNT(*), 25 FROM Reserva
UNION ALL SELECT 'Boleto', COUNT(*), 15 FROM Boleto
UNION ALL SELECT 'CheckIn', COUNT(*), 3 FROM CheckIn
UNION ALL SELECT 'Embarque', COUNT(*), 2 FROM Embarque
UNION ALL SELECT 'Equipaje', COUNT(*), 10 FROM Equipaje
UNION ALL SELECT 'Empleado', COUNT(*), 8 FROM Empleado
UNION ALL SELECT 'TripulacionVuelo', COUNT(*), 12 FROM TripulacionVuelo
UNION ALL SELECT 'AspNetRoles', COUNT(*), 0 FROM AspNetRoles
UNION ALL SELECT 'AspNetUsers', COUNT(*), 0 FROM AspNetUsers
UNION ALL SELECT 'AspNetRoleClaims', COUNT(*), 0 FROM AspNetRoleClaims
UNION ALL SELECT 'AspNetUserClaims', COUNT(*), 0 FROM AspNetUserClaims
UNION ALL SELECT 'AspNetUserLogins', COUNT(*), 0 FROM AspNetUserLogins
UNION ALL SELECT 'AspNetUserRoles', COUNT(*), 0 FROM AspNetUserRoles
UNION ALL SELECT 'AspNetUserTokens', COUNT(*), 0 FROM AspNetUserTokens;
```

Cada valor de `Filas` debe coincidir con `Esperadas` después de instalar los tres scripts sobre una base vacía. Las 12 filas de `TripulacionVuelo` corresponden a tres empleados asignados a cada uno de cuatro vuelos. Las tablas de Identity permanecen vacías hasta crear usuarios, roles u otras entradas de autenticación desde su flujo correspondiente; la semilla no contiene cuentas ni contraseñas.

Los conteos cambian cuando el equipo usa la aplicación o agrega registros. Para comprobar también estados, asientos, fechas y relaciones de la semilla recién instalada, ejecutar [03_seed_validation.sql](../tests/database/03_seed_validation.sql); sus 32 comprobaciones deben indicar PASS. La validación temporal se realiza inmediatamente después de la primera carga. Más detalles en [DATOS_PRUEBA.md](../docs/DATOS_PRUEBA.md).

## Reiniciar la base local

**ADVERTENCIA: `99_reset.sql` elimina toda `skyport_db`, incluidas las tablas operativas, las tablas de Identity y todos sus datos. Solo se usa sobre bases locales.**

Desde Rider, ejecutar completo [99_reset.sql](99_reset.sql) sobre la conexión al servidor local. Después del borrado puede ser necesario refrescar la fuente de datos y seleccionar una base existente o la conexión al servidor para volver a crear `skyport_db`.

Desde CMD:

```bat
mysql -u root -p < database/99_reset.sql
mysql -u root -p < database/01_schema.sql
mysql -u root -p < database/02_identity.sql
mysql -u root -p < database/03_seed.sql
```

Desde PowerShell:

```powershell
Get-Content -Raw database/99_reset.sql | mysql -u root -p
Get-Content -Raw database/01_schema.sql | mysql -u root -p
Get-Content -Raw database/02_identity.sql | mysql -u root -p
Get-Content -Raw database/03_seed.sql | mysql -u root -p
```

Después del reset siempre se vuelven a ejecutar **01_schema.sql → 02_identity.sql → 03_seed.sql**, en ese orden. Verificar nuevamente las tablas y los conteos.

## Cuando cambia el esquema

1. Editar `database/01_schema.sql` para reflejar el nuevo modelo y ajustar `03_seed.sql` si el cambio afecta los datos de prueba. Actualizar también el [ERD](../docs/ERD.md) y los conteos de esta guía si corresponde.
2. Avisar al grupo y recrear la base local con `99_reset.sql`, seguido de `01_schema.sql`, `02_identity.sql` y `03_seed.sql`.
3. Verificar los conteos y la cadena de conexión local.
4. Sebas o Johan ejecutan `tools/scaffold.ps1` para regenerar las entidades operativas y `SkyPortContext` desde la base ya preparada. No editar a mano los archivos generados ni crear migraciones.

**Estado actual:** `tools/scaffold.ps1` todavía no está en el repositorio; corresponde a S07. Una vez que S07 lo agregue, ejecutarlo desde la raíz con PowerShell:

```powershell
.\tools\scaffold.ps1
```

Si cambia el modelo de Identity, regenerar además su script con el procedimiento de [IDENTITY.md](../docs/IDENTITY.md). Ese contexto tiene su propio modelo y no se sustituye por las entidades operativas del scaffold.
