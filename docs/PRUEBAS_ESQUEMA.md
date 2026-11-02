# J02: validación del esquema MySQL

`database/01_schema.sql` contiene exclusivamente las 14 tablas operativas documentadas en [ERD.md](ERD.md). No contiene usuarios, tablas de Identity ni datos de semilla. Se requiere MySQL 8.0.16 o superior para aplicar los CHECK; MariaDB de XAMPP no sustituye esta validación. El prompt J02 exige MySQL y tiene prioridad para este script sobre la referencia a SQL Server de `CONVENCIONES.md`.

## Repetir la validación

Usar una instancia de MySQL de pruebas vacía. El script utiliza el nombre fijo `skyport_db`; no ejecutar las pruebas sobre una base con información del equipo. `IF NOT EXISTS` permite repetir la creación, pero no compara ni modifica estructuras preexistentes.

Desde la raíz del repositorio, en una terminal que admita redirección de entrada (por ejemplo, CMD en Windows):

```bat
mysql -u root -p < database/01_schema.sql
mysql -u root -p < database/01_schema.sql
mysql -u root -p --force --batch < tests/database/01_schema_constraints.sql
```

En PowerShell, que no admite `<` para este uso, se puede enviar el contenido por la entrada estándar:

```powershell
Get-Content -Raw database/01_schema.sql | mysql -u root -p
Get-Content -Raw database/01_schema.sql | mysql -u root -p
Get-Content -Raw tests/database/01_schema_constraints.sql | mysql -u root -p --force --batch
```

Agregar `--host=127.0.0.1 --port=PUERTO` si la instancia usa otro puerto. La contraseña se solicita de forma interactiva; no se guarda en los archivos. En las dos ejecuciones del esquema, el código de salida debe ser 0. La segunda puede producir avisos por objetos existentes.

El archivo de pruebas usa una transacción, inserta registros de apoyo y termina con `ROLLBACK`. No deja filas, aunque los contadores AUTO_INCREMENT pueden avanzar. `--force` se usa **solo en las pruebas** para continuar después de los cinco rechazos esperados. No basta con revisar el código de salida: debe haber exactamente los cinco errores siguientes, cinco resultados `PASS:` y ningún `FAIL:` ni otro error.

## Los cinco INSERT que deben fallar

Los INSERT completos y sus datos de apoyo están en [01_schema_constraints.sql](../tests/database/01_schema_constraints.sql).

| Prueba | INSERT inválido | Restricción esperada | Error MySQL |
| --- | --- | --- | --- |
| 1 | Aeronave con `Capacidad = 0` y aerolínea válida. | `CK_Aeronave_Capacidad` | 3819 |
| 2 | Aeronave con `AerolineaId = -1`, inexistente en la instancia vacía. | `FK_Aeronave_AerolineaId` | 1452 |
| 3 | Reserva con `Estado = 'Pendiente'`. | `CK_Reserva_Estado` | 3819 |
| 4 | Segunda reserva `Confirmada` del asiento `1A` del mismo vuelo, para otro pasajero. | `UQ_Reserva_VueloId_AsientoActivo` | 1062 |
| 5 | Segunda reserva `Confirmada` del mismo pasajero y vuelo, con asiento diferente (`1B`). | `UQ_Reserva_VueloId_PasajeroActivoId` | 1062 |

Después de esos rechazos se cancela la primera reserva: ambas columnas generadas deben quedar en NULL. Se confirma exitosamente `1A` para el segundo pasajero y `1B` para el pasajero original. También se comprueban los cinco defaults pedidos: `Activa`, `Disponible` y `Activo` iguales a 1, y `FechaReserva` y `FechaEmision` no nulas al omitirlas en los INSERT. Finalmente se verifica que las 14 tablas queden sin filas después del rollback.

## Resultado de la ejecución de J02

Validado el 2 de noviembre de 2026 en MySQL Community Server 8.4.13, en una instancia temporal vacía ligada a `127.0.0.1:33317`, sin instalar servicios ni modificar XAMPP:

- Dos ejecuciones consecutivas de `01_schema.sql`: código 0 en ambas.
- 14 tablas InnoDB con charset utf8mb4 y collation utf8mb4_unicode_ci.
- 14 claves primarias, 17 restricciones UNIQUE, 15 claves foráneas y 12 CHECK.
- Las 15 claves foráneas tienen `ON DELETE NO ACTION`.
- `AsientoActivo` y `PasajeroActivoId` son columnas `STORED GENERATED`.
- Los cinco INSERT inválidos producen los errores y restricciones de la tabla anterior.
- Cinco controles positivos con resultado PASS: rechazos sin filas añadidas, columnas generadas NULL, reutilización de asiento y pasajero, defaults y rollback sin filas persistidas.

Los nombres `PK_...` se declaran en el SQL, pero MySQL conserva las claves primarias con el nombre interno `PRIMARY`; es una particularidad del motor documentada en [CREATE TABLE de MySQL](https://dev.mysql.com/doc/refman/8.0/en/create-table.html). Las demás restricciones conservan los nombres explícitos.
