# J04: datos ficticios para pruebas

`database/03_seed.sql` carga datos de demostración sobre `skyport_db` después de ejecutar `01_schema.sql` y `02_identity.sql`. No agrega usuarios ni contraseñas de Identity. Las aerolíneas y códigos son los solicitados; nombres de pasajeros y empleados, documentos, matrículas, números de vuelo y operaciones son ficticios. Los documentos y matrículas llevan el prefijo `FIC-J04-`; los correos usan `example.invalid`.

## Cantidades

Estas cantidades corresponden a una base con las tablas operativas vacías antes de la primera carga:

| Tabla | Filas |
| --- | ---: |
| Aerolinea | 4 |
| Aeronave | 8 |
| Terminal | 2 |
| Puerta | 10 |
| Ruta | 10 |
| Vuelo | 12 |
| Pasajero | 20 |
| Reserva | 25 |
| Boleto | 15 |
| CheckIn | 3 |
| Embarque | 2 |
| Equipaje | 10 |
| Empleado | 8 |
| TripulacionVuelo | 12 asignaciones para 4 vuelos |

Las aerolíneas son LATAM (`LA`), Sky Airline (`H2`), JetSMART (`JA`) y Avianca (`AV`). Cada una tiene dos aeronaves, con capacidades entre 120 y 186. Se incluyen aeronaves en mantenimiento y fuera de servicio, pero los vuelos usan únicamente aeronaves operativas.

Las terminales Nacional e Internacional tienen cinco puertas cada una. Hay rutas de ida y vuelta entre LIM y CUZ, AQP, PIU e IQT, además de LIM–SCL y BOG–LIM. Las duraciones son valores de demostración.

## Fechas y repetibilidad

El script establece temporalmente la zona de la sesión en `-05:00` para calcular fechas locales de Lima con `NOW()`, y restaura la zona al finalizar. Las salidas de los vuelos nuevos se calculan con `DATE_ADD(NOW(), INTERVAL n DAY)`, entre 1 y 12 días después de la carga; las llegadas suman la duración de su ruta. Los estados variados y los check-in y embarques futuros son escenarios sintéticos para probar pantallas, no un historial de operaciones reales.

Cada INSERT usa SELECT con filas de apoyo y un `WHERE NOT EXISTS` correlacionado. No se usa `INSERT IGNORE`, de modo que un error de CHECK o FK se informa. Los vínculos se resuelven por códigos, documentos o claves compuestas; no se insertan ni se suponen IDs numéricos concretos. La carga se ejecuta dentro de una transacción.

Los números de vuelo de esta demostración (`LA9001`–`LA9003`, `H29001`–`H29003`, `JA9001`–`JA9003` y `AV9001`–`AV9003`) se consideran identificadores reservados de la semilla. Se comprueba su existencia por número, sin incluir la fecha, para que cambiar `NOW()` no genere más vuelos al repetir la carga. Las repeticiones no actualizan datos existentes ni reprograman sus fechas; las fechas relativas se asignan al insertar cada vuelo por primera vez.

## Escenarios incluidos

- `LA9001` está Embarcando. Sus cuatro reservas tienen asientos `1A`–`1D`; dos boletos están Abordado, uno CheckInRealizado y uno Emitido. Los tres primeros tienen check-in y los dos primeros tienen embarque en la puerta del vuelo.
- `LA9002` permite probar boletos Emitido y reservas confirmadas sin duplicar asiento ni pasajero dentro del vuelo.
- `H29001` y `LA9003` están Retrasado; `AV9003` está Despegado.
- `JA9002` está Cancelado, con una reserva Cancelada y su boleto Cancelado. Sus columnas generadas de reserva activa son NULL.
- Hay 24 reservas Confirmada y una Cancelada. Los 25 asientos son distintos dentro de cada vuelo, incluso contando la cancelada.
- Los diez equipajes pesan entre 8,40 y 32 kg. Los Cargado pertenecen a boletos con check-in y los demás están Registrado; ningún boleto tiene más de dos maletas.
- Hay dos pilotos, dos copilotos, dos tripulantes de cabina y dos empleados de tierra. Los cuatro primeros vuelos tienen piloto, copiloto y tripulante asignados; el personal de tierra figura en los check-in.

## Ejecución y validación

En CMD, desde la raíz del repositorio y sobre una instancia de pruebas preparada con J02 y J03:

```bat
mysql -u root -p < database/03_seed.sql
mysql -u root -p < database/03_seed.sql
mysql -u root -p --batch < tests/database/03_seed_validation.sql
```

En PowerShell:

```powershell
Get-Content -Raw database/03_seed.sql | mysql -u root -p
Get-Content -Raw database/03_seed.sql | mysql -u root -p
Get-Content -Raw tests/database/03_seed_validation.sql | mysql -u root -p --batch
```

En Workbench, ejecutar completo `03_seed.sql` dos veces y luego `tests/database/03_seed_validation.sql`. Todas las comprobaciones deben indicar PASS. La comprobación temporal está pensada para realizarse inmediatamente después de la primera carga; una base antigua conserva las fechas originales.

Validado el 2 de noviembre de 2026 en MySQL Community Server 8.4.13, sobre la instancia temporal de J02 y J03:

- Dos ejecuciones sin errores y con las cantidades indicadas.
- En la segunda ejecución se adelantó el reloj de esa sesión un día con `SET timestamp = UNIX_TIMESTAMP() + 86400` para comprobar que el cambio de `NOW()` tampoco duplica vuelos.
- Se compararon los volcados de datos de las 21 tablas antes y después de la segunda ejecución: todas las filas permanecieron idénticas.
- Las 32 comprobaciones de cantidades, fechas, capacidades, asientos, estados, equipajes y tripulaciones pasaron antes y después de la segunda carga.
