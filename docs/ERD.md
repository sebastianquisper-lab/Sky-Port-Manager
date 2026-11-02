# Modelo entidad-relación

Fuente: sección 3, «Modelo de datos», de `SkyPort_Manager_plan_de_prompts_y_commits_corregido (2).docx`, ya que `CONVENCIONES.md` no contiene esa sección. Se incluyen únicamente las 14 tablas de J01. Los tipos se conservan en MySQL tal como aparecen en la fuente, aunque las convenciones actuales del repositorio indican SQL Server; este documento no cambia el proveedor del proyecto.

Las PK simples son `int AUTO_INCREMENT`; `TripulacionVuelo` tiene PK compuesta. La fuente solo marca explícitamente como anulables las columnas indicadas con `null`; para las demás columnas ordinarias se interpreta `NOT NULL`. Las columnas generadas de `Reserva` pueden producir `NULL`. PK = clave primaria; FK = clave foránea; UK = pertenencia a una clave única. Una marca UK en varias columnas no implica unicidad individual: las combinaciones exactas figuran debajo de cada tabla. No se agregan reglas de borrado ni valores predeterminados que la fuente no especifique. Las fechas se guardan en hora local de Lima.

## Diagrama

Cada relación muestra también la opcionalidad: `||` = uno obligatorio, `o|` = cero o uno y `o{` = cero o muchos. Una FK única permite como máximo un registro hijo, pero no obliga al padre a tenerlo.

```mermaid
erDiagram
    Aerolinea ||--o{ Aeronave : posee
    Terminal ||--o{ Puerta : contiene
    Ruta ||--o{ Vuelo : define
    Aeronave ||--o{ Vuelo : realiza
    Puerta o|--o{ Vuelo : atiende
    Pasajero ||--o{ Reserva : realiza
    Vuelo ||--o{ Reserva : recibe
    Reserva ||--o| Boleto : tiene
    Boleto ||--o| CheckIn : registra
    Empleado o|--o{ CheckIn : verifica
    Boleto ||--o| Embarque : registra
    Puerta o|--o{ Embarque : recibe
    Boleto ||--o{ Equipaje : incluye
    Vuelo ||--o{ TripulacionVuelo : asigna
    Empleado ||--o{ TripulacionVuelo : integra

    Aerolinea {
        int AerolineaId PK
        varchar(100) Nombre UK
        varchar(2) CodigoIata UK
        varchar(60) Pais
        tinyint(1) Activa
    }
    Aeronave {
        int AeronaveId PK
        int AerolineaId FK
        varchar(20) Matricula UK
        varchar(60) Modelo
        int Capacidad
        varchar(20) Estado
    }
    Terminal {
        int TerminalId PK
        varchar(50) Nombre UK
    }
    Puerta {
        int PuertaId PK
        int TerminalId FK, UK
        varchar(10) Codigo UK
        tinyint(1) Disponible
    }
    Ruta {
        int RutaId PK
        varchar(3) CodigoOrigen UK
        varchar(80) CiudadOrigen
        varchar(3) CodigoDestino UK
        varchar(80) CiudadDestino
        int DuracionMinutos
    }
    Vuelo {
        int VueloId PK
        varchar(10) NumeroVuelo UK
        int RutaId FK
        int AeronaveId FK
        int PuertaId FK "nullable"
        datetime FechaSalida UK
        datetime FechaLlegada
        varchar(20) Estado
    }
    Pasajero {
        int PasajeroId PK
        varchar(20) TipoDocumento UK
        varchar(20) NumeroDocumento UK
        varchar(80) Nombres
        varchar(80) Apellidos
        date FechaNacimiento
        varchar(60) Nacionalidad
        varchar(120) Email "nullable"
        varchar(20) Telefono "nullable"
    }
    Reserva {
        int ReservaId PK
        varchar(10) Codigo UK
        int PasajeroId FK
        int VueloId FK, UK
        varchar(4) Asiento
        datetime FechaReserva
        varchar(20) Estado
        varchar(4) AsientoActivo UK "generada STORED, nullable"
        int PasajeroActivoId UK "generada STORED, nullable"
    }
    Boleto {
        int BoletoId PK
        int ReservaId FK, UK
        varchar(14) CodigoBoleto UK
        datetime FechaEmision
        varchar(20) Estado
    }
    CheckIn {
        int CheckInId PK
        int BoletoId FK, UK
        int EmpleadoId FK "nullable"
        datetime FechaHora
        tinyint(1) DocumentoVerificado
    }
    Embarque {
        int EmbarqueId PK
        int BoletoId FK, UK
        int PuertaId FK "nullable"
        datetime FechaHora
    }
    Equipaje {
        int EquipajeId PK
        int BoletoId FK
        varchar(14) Codigo UK
        decimal(5,2) PesoKg
        varchar(20) Estado
    }
    Empleado {
        int EmpleadoId PK
        varchar(20) NumeroDocumento UK
        varchar(80) Nombres
        varchar(80) Apellidos
        varchar(40) Cargo
        varchar(120) Email "nullable"
        tinyint(1) Activo
    }
    TripulacionVuelo {
        int VueloId PK, FK
        int EmpleadoId PK, FK
        varchar(30) RolABordo
    }
```

## Diccionario de datos

### Aerolinea

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| AerolineaId | int | No | PK autoincremental de la aerolínea. |
| Nombre | varchar(100) | No | Nombre único. |
| CodigoIata | varchar(2) | No | Código IATA único. |
| Pais | varchar(60) | No | País de la aerolínea. |
| Activa | tinyint(1) | No | Indicador de actividad. |

Restricciones: UK `Nombre`; UK `CodigoIata`.

### Aeronave

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| AeronaveId | int | No | PK autoincremental de la aeronave. |
| AerolineaId | int | No | FK a Aerolinea.AerolineaId. |
| Matricula | varchar(20) | No | Matrícula única. |
| Modelo | varchar(60) | No | Modelo de aeronave. |
| Capacidad | int | No | Cantidad de asientos, entre 1 y 900. |
| Estado | varchar(20) | No | Estado operativo. |

Restricciones: UK `Matricula`; CHECK `Capacidad BETWEEN 1 AND 900`; CHECK `Estado IN ('Operativa', 'Mantenimiento', 'FueraDeServicio')`.

### Terminal

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| TerminalId | int | No | PK autoincremental de la terminal. |
| Nombre | varchar(50) | No | Nombre único. |

Restricciones: UK `Nombre`.

### Puerta

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| PuertaId | int | No | PK autoincremental de la puerta. |
| TerminalId | int | No | FK a Terminal.TerminalId. |
| Codigo | varchar(10) | No | Código de puerta dentro de su terminal. |
| Disponible | tinyint(1) | No | Indicador de disponibilidad. |

Restricciones: UK compuesta `(TerminalId, Codigo)`.

### Ruta

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| RutaId | int | No | PK autoincremental de la ruta. |
| CodigoOrigen | varchar(3) | No | Código del origen. |
| CiudadOrigen | varchar(80) | No | Ciudad del origen. |
| CodigoDestino | varchar(3) | No | Código del destino. |
| CiudadDestino | varchar(80) | No | Ciudad del destino. |
| DuracionMinutos | int | No | Duración prevista en minutos. |

Restricciones: UK compuesta `(CodigoOrigen, CodigoDestino)`; CHECK `CodigoOrigen <> CodigoDestino`; CHECK `DuracionMinutos > 0`.

### Vuelo

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| VueloId | int | No | PK autoincremental del vuelo. |
| NumeroVuelo | varchar(10) | No | Número del vuelo. |
| RutaId | int | No | FK a Ruta.RutaId. |
| AeronaveId | int | No | FK a Aeronave.AeronaveId. |
| PuertaId | int | Sí | FK a Puerta.PuertaId; permite vuelo sin puerta asignada. |
| FechaSalida | datetime | No | Fecha y hora de salida. |
| FechaLlegada | datetime | No | Fecha y hora de llegada. |
| Estado | varchar(20) | No | Estado del vuelo. |

Restricciones: UK compuesta `(NumeroVuelo, FechaSalida)`; CHECK `FechaLlegada > FechaSalida`; CHECK `Estado IN ('Programado', 'Embarcando', 'Retrasado', 'Despegado', 'Cancelado')`. Índices en `FechaSalida`, `PuertaId` y `AeronaveId`.

### Pasajero

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| PasajeroId | int | No | PK autoincremental del pasajero. |
| TipoDocumento | varchar(20) | No | Tipo del documento de identidad. |
| NumeroDocumento | varchar(20) | No | Número del documento de identidad. |
| Nombres | varchar(80) | No | Nombres del pasajero. |
| Apellidos | varchar(80) | No | Apellidos del pasajero. |
| FechaNacimiento | date | No | Fecha de nacimiento. |
| Nacionalidad | varchar(60) | No | Nacionalidad del pasajero. |
| Email | varchar(120) | Sí | Correo de contacto opcional. |
| Telefono | varchar(20) | Sí | Teléfono de contacto opcional. |

Restricciones: UK compuesta `(TipoDocumento, NumeroDocumento)`; CHECK `TipoDocumento IN ('DNI', 'Pasaporte', 'CE')`.

### Reserva

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| ReservaId | int | No | PK autoincremental de la reserva. |
| Codigo | varchar(10) | No | Código único de reserva. |
| PasajeroId | int | No | FK a Pasajero.PasajeroId. |
| VueloId | int | No | FK a Vuelo.VueloId. |
| Asiento | varchar(4) | No | Asiento reservado. |
| FechaReserva | datetime | No | Fecha de reserva; DEFAULT CURRENT_TIMESTAMP. |
| Estado | varchar(20) | No | Estado de la reserva. |
| AsientoActivo | varchar(4) | Sí | Generada STORED: IF(Estado = 'Confirmada', Asiento, NULL). |
| PasajeroActivoId | int | Sí | Generada STORED: IF(Estado = 'Confirmada', PasajeroId, NULL); no es una FK adicional. |

Restricciones: UK `Codigo`; CHECK `Estado IN ('Confirmada', 'Cancelada')`; UK compuesta `(VueloId, AsientoActivo)`; UK compuesta `(VueloId, PasajeroActivoId)`. Las columnas generadas impiden duplicar asiento o pasajero en reservas confirmadas del mismo vuelo y permiten conservar reservas canceladas gracias a los valores NULL.

### Boleto

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| BoletoId | int | No | PK autoincremental del boleto. |
| ReservaId | int | No | FK única a Reserva.ReservaId. |
| CodigoBoleto | varchar(14) | No | Código único de boleto. |
| FechaEmision | datetime | No | Fecha de emisión; DEFAULT CURRENT_TIMESTAMP. |
| Estado | varchar(20) | No | Estado del boleto. |

Restricciones: UK `ReservaId`; UK `CodigoBoleto`; CHECK `Estado IN ('Emitido', 'CheckInRealizado', 'Abordado', 'Cancelado')`. Cada boleto tiene una reserva; cada reserva puede tener cero o un boleto.

### CheckIn

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| CheckInId | int | No | PK autoincremental del check-in. |
| BoletoId | int | No | FK única a Boleto.BoletoId. |
| EmpleadoId | int | Sí | FK opcional a Empleado.EmpleadoId. |
| FechaHora | datetime | No | Fecha y hora del check-in. |
| DocumentoVerificado | tinyint(1) | No | Indicador de verificación del documento. |

Restricciones: UK `BoletoId`; un boleto puede tener cero o un check-in.

### Embarque

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| EmbarqueId | int | No | PK autoincremental del embarque. |
| BoletoId | int | No | FK única a Boleto.BoletoId. |
| PuertaId | int | Sí | FK opcional a Puerta.PuertaId. |
| FechaHora | datetime | No | Fecha y hora del embarque. |

Restricciones: UK `BoletoId`; un boleto puede tener cero o un embarque.

### Equipaje

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| EquipajeId | int | No | PK autoincremental del equipaje. |
| BoletoId | int | No | FK a Boleto.BoletoId. |
| Codigo | varchar(14) | No | Código único de equipaje. |
| PesoKg | decimal(5,2) | No | Peso en kilogramos, mayor que 0 y hasta 32. |
| Estado | varchar(20) | No | Estado del equipaje. |

Restricciones: UK `Codigo`; CHECK `PesoKg > 0 AND PesoKg <= 32`; CHECK `Estado IN ('Registrado', 'Cargado', 'Entregado')`.

### Empleado

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| EmpleadoId | int | No | PK autoincremental del empleado. |
| NumeroDocumento | varchar(20) | No | Número único de documento. |
| Nombres | varchar(80) | No | Nombres del empleado. |
| Apellidos | varchar(80) | No | Apellidos del empleado. |
| Cargo | varchar(40) | No | Cargo del empleado. |
| Email | varchar(120) | Sí | Correo de contacto opcional. |
| Activo | tinyint(1) | No | Indicador de actividad. |

Restricciones: UK `NumeroDocumento`; CHECK `Cargo IN ('Piloto', 'Copiloto', 'TripulanteCabina', 'PersonalTierra', 'Administrativo')`.

### TripulacionVuelo

| Columna | Tipo | Acepta nulos | Descripción |
| --- | --- | --- | --- |
| VueloId | int | No | Parte de la PK compuesta; FK a Vuelo.VueloId. |
| EmpleadoId | int | No | Parte de la PK compuesta; FK a Empleado.EmpleadoId. |
| RolABordo | varchar(30) | No | Rol del empleado en ese vuelo. |

Restricciones: PK compuesta `(VueloId, EmpleadoId)`, sin AUTO_INCREMENT. Resuelve la relación muchos a muchos entre Vuelo y Empleado; un empleado aparece como máximo una vez por vuelo. La fuente no define CHECK para `RolABordo`.

## Tercera forma normal

El modelo lógico sigue la tercera forma normal al separar los datos de cada entidad, usar atributos atómicos y representar las asociaciones mediante claves foráneas: por ejemplo, una reserva referencia al pasajero y al vuelo sin repetir sus datos. En `TripulacionVuelo`, `RolABordo` depende de la combinación completa de vuelo y empleado. Esta justificación supone únicamente las dependencias declaradas en la fuente; si un código de origen o destino determina por sí solo una ciudad, `Ruta` tendría una dependencia transitiva que requeriría revisión. Además, el esquema físico conserva por exigencia de la fuente las columnas generadas `AsientoActivo` y `PasajeroActivoId`: son redundancia calculada para aplicar unicidad condicional, por lo que no sería exacto afirmar que todas las columnas almacenadas cumplen estrictamente la 3FN. No se modifican tablas ni columnas para ocultar estas salvedades.
