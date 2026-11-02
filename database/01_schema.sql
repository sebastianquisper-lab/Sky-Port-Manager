CREATE DATABASE IF NOT EXISTS skyport_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE skyport_db;

-- MySQL 8.0.16+; modelo de las 14 tablas operativas de docs/ERD.md.
-- MySQL registra las claves primarias como PRIMARY, aunque se nombre CONSTRAINT PK_.
-- IF NOT EXISTS permite repetir la creacion; no actualiza tablas ya existentes.

CREATE TABLE IF NOT EXISTS Aerolinea (
    AerolineaId int NOT NULL AUTO_INCREMENT,
    Nombre varchar(100) NOT NULL,
    CodigoIata varchar(2) NOT NULL,
    Pais varchar(60) NOT NULL,
    Activa tinyint(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_Aerolinea_AerolineaId PRIMARY KEY (AerolineaId),
    CONSTRAINT UQ_Aerolinea_Nombre UNIQUE (Nombre),
    CONSTRAINT UQ_Aerolinea_CodigoIata UNIQUE (CodigoIata)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Aeronave (
    AeronaveId int NOT NULL AUTO_INCREMENT,
    AerolineaId int NOT NULL,
    Matricula varchar(20) NOT NULL,
    Modelo varchar(60) NOT NULL,
    Capacidad int NOT NULL,
    Estado varchar(20) NOT NULL,
    CONSTRAINT PK_Aeronave_AeronaveId PRIMARY KEY (AeronaveId),
    CONSTRAINT FK_Aeronave_AerolineaId FOREIGN KEY (AerolineaId)
        REFERENCES Aerolinea (AerolineaId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Aeronave_Matricula UNIQUE (Matricula),
    CONSTRAINT CK_Aeronave_Capacidad CHECK (Capacidad BETWEEN 1 AND 900),
    CONSTRAINT CK_Aeronave_Estado CHECK (Estado IN ('Operativa', 'Mantenimiento', 'FueraDeServicio')),
    INDEX IX_Aeronave_AerolineaId (AerolineaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Terminal (
    TerminalId int NOT NULL AUTO_INCREMENT,
    Nombre varchar(50) NOT NULL,
    CONSTRAINT PK_Terminal_TerminalId PRIMARY KEY (TerminalId),
    CONSTRAINT UQ_Terminal_Nombre UNIQUE (Nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Puerta (
    PuertaId int NOT NULL AUTO_INCREMENT,
    TerminalId int NOT NULL,
    Codigo varchar(10) NOT NULL,
    Disponible tinyint(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_Puerta_PuertaId PRIMARY KEY (PuertaId),
    CONSTRAINT FK_Puerta_TerminalId FOREIGN KEY (TerminalId)
        REFERENCES Terminal (TerminalId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Puerta_TerminalId_Codigo UNIQUE (TerminalId, Codigo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Ruta (
    RutaId int NOT NULL AUTO_INCREMENT,
    CodigoOrigen varchar(3) NOT NULL,
    CiudadOrigen varchar(80) NOT NULL,
    CodigoDestino varchar(3) NOT NULL,
    CiudadDestino varchar(80) NOT NULL,
    DuracionMinutos int NOT NULL,
    CONSTRAINT PK_Ruta_RutaId PRIMARY KEY (RutaId),
    CONSTRAINT UQ_Ruta_CodigoOrigen_CodigoDestino UNIQUE (CodigoOrigen, CodigoDestino),
    CONSTRAINT CK_Ruta_CodigoOrigen_CodigoDestino CHECK (CodigoOrigen <> CodigoDestino),
    CONSTRAINT CK_Ruta_DuracionMinutos CHECK (DuracionMinutos > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Vuelo (
    VueloId int NOT NULL AUTO_INCREMENT,
    NumeroVuelo varchar(10) NOT NULL,
    RutaId int NOT NULL,
    AeronaveId int NOT NULL,
    PuertaId int NULL,
    FechaSalida datetime NOT NULL,
    FechaLlegada datetime NOT NULL,
    Estado varchar(20) NOT NULL,
    CONSTRAINT PK_Vuelo_VueloId PRIMARY KEY (VueloId),
    CONSTRAINT FK_Vuelo_RutaId FOREIGN KEY (RutaId)
        REFERENCES Ruta (RutaId) ON DELETE NO ACTION,
    CONSTRAINT FK_Vuelo_AeronaveId FOREIGN KEY (AeronaveId)
        REFERENCES Aeronave (AeronaveId) ON DELETE NO ACTION,
    CONSTRAINT FK_Vuelo_PuertaId FOREIGN KEY (PuertaId)
        REFERENCES Puerta (PuertaId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Vuelo_NumeroVuelo_FechaSalida UNIQUE (NumeroVuelo, FechaSalida),
    CONSTRAINT CK_Vuelo_FechaLlegada_FechaSalida CHECK (FechaLlegada > FechaSalida),
    CONSTRAINT CK_Vuelo_Estado CHECK (Estado IN ('Programado', 'Embarcando', 'Retrasado', 'Despegado', 'Cancelado')),
    INDEX IX_Vuelo_RutaId (RutaId),
    INDEX IX_Vuelo_FechaSalida (FechaSalida),
    INDEX IX_Vuelo_PuertaId (PuertaId),
    INDEX IX_Vuelo_AeronaveId (AeronaveId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Pasajero (
    PasajeroId int NOT NULL AUTO_INCREMENT,
    TipoDocumento varchar(20) NOT NULL,
    NumeroDocumento varchar(20) NOT NULL,
    Nombres varchar(80) NOT NULL,
    Apellidos varchar(80) NOT NULL,
    FechaNacimiento date NOT NULL,
    Nacionalidad varchar(60) NOT NULL,
    Email varchar(120) NULL,
    Telefono varchar(20) NULL,
    CONSTRAINT PK_Pasajero_PasajeroId PRIMARY KEY (PasajeroId),
    CONSTRAINT UQ_Pasajero_TipoDocumento_NumeroDocumento UNIQUE (TipoDocumento, NumeroDocumento),
    CONSTRAINT CK_Pasajero_TipoDocumento CHECK (TipoDocumento IN ('DNI', 'Pasaporte', 'CE'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Reserva (
    ReservaId int NOT NULL AUTO_INCREMENT,
    Codigo varchar(10) NOT NULL,
    PasajeroId int NOT NULL,
    VueloId int NOT NULL,
    Asiento varchar(4) NOT NULL,
    FechaReserva datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Estado varchar(20) NOT NULL,
    AsientoActivo varchar(4) GENERATED ALWAYS AS (IF(Estado = 'Confirmada', Asiento, NULL)) STORED,
    PasajeroActivoId int GENERATED ALWAYS AS (IF(Estado = 'Confirmada', PasajeroId, NULL)) STORED,
    CONSTRAINT PK_Reserva_ReservaId PRIMARY KEY (ReservaId),
    CONSTRAINT FK_Reserva_PasajeroId FOREIGN KEY (PasajeroId)
        REFERENCES Pasajero (PasajeroId) ON DELETE NO ACTION,
    CONSTRAINT FK_Reserva_VueloId FOREIGN KEY (VueloId)
        REFERENCES Vuelo (VueloId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Reserva_Codigo UNIQUE (Codigo),
    CONSTRAINT UQ_Reserva_VueloId_AsientoActivo UNIQUE (VueloId, AsientoActivo),
    CONSTRAINT UQ_Reserva_VueloId_PasajeroActivoId UNIQUE (VueloId, PasajeroActivoId),
    CONSTRAINT CK_Reserva_Estado CHECK (Estado IN ('Confirmada', 'Cancelada')),
    INDEX IX_Reserva_PasajeroId (PasajeroId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Boleto (
    BoletoId int NOT NULL AUTO_INCREMENT,
    ReservaId int NOT NULL,
    CodigoBoleto varchar(14) NOT NULL,
    FechaEmision datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Estado varchar(20) NOT NULL,
    CONSTRAINT PK_Boleto_BoletoId PRIMARY KEY (BoletoId),
    CONSTRAINT FK_Boleto_ReservaId FOREIGN KEY (ReservaId)
        REFERENCES Reserva (ReservaId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Boleto_ReservaId UNIQUE (ReservaId),
    CONSTRAINT UQ_Boleto_CodigoBoleto UNIQUE (CodigoBoleto),
    CONSTRAINT CK_Boleto_Estado CHECK (Estado IN ('Emitido', 'CheckInRealizado', 'Abordado', 'Cancelado'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Empleado (
    EmpleadoId int NOT NULL AUTO_INCREMENT,
    NumeroDocumento varchar(20) NOT NULL,
    Nombres varchar(80) NOT NULL,
    Apellidos varchar(80) NOT NULL,
    Cargo varchar(40) NOT NULL,
    Email varchar(120) NULL,
    Activo tinyint(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_Empleado_EmpleadoId PRIMARY KEY (EmpleadoId),
    CONSTRAINT UQ_Empleado_NumeroDocumento UNIQUE (NumeroDocumento),
    CONSTRAINT CK_Empleado_Cargo CHECK (Cargo IN ('Piloto', 'Copiloto', 'TripulanteCabina', 'PersonalTierra', 'Administrativo'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS CheckIn (
    CheckInId int NOT NULL AUTO_INCREMENT,
    BoletoId int NOT NULL,
    EmpleadoId int NULL,
    FechaHora datetime NOT NULL,
    DocumentoVerificado tinyint(1) NOT NULL,
    CONSTRAINT PK_CheckIn_CheckInId PRIMARY KEY (CheckInId),
    CONSTRAINT FK_CheckIn_BoletoId FOREIGN KEY (BoletoId)
        REFERENCES Boleto (BoletoId) ON DELETE NO ACTION,
    CONSTRAINT FK_CheckIn_EmpleadoId FOREIGN KEY (EmpleadoId)
        REFERENCES Empleado (EmpleadoId) ON DELETE NO ACTION,
    CONSTRAINT UQ_CheckIn_BoletoId UNIQUE (BoletoId),
    INDEX IX_CheckIn_EmpleadoId (EmpleadoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Embarque (
    EmbarqueId int NOT NULL AUTO_INCREMENT,
    BoletoId int NOT NULL,
    PuertaId int NULL,
    FechaHora datetime NOT NULL,
    CONSTRAINT PK_Embarque_EmbarqueId PRIMARY KEY (EmbarqueId),
    CONSTRAINT FK_Embarque_BoletoId FOREIGN KEY (BoletoId)
        REFERENCES Boleto (BoletoId) ON DELETE NO ACTION,
    CONSTRAINT FK_Embarque_PuertaId FOREIGN KEY (PuertaId)
        REFERENCES Puerta (PuertaId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Embarque_BoletoId UNIQUE (BoletoId),
    INDEX IX_Embarque_PuertaId (PuertaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS Equipaje (
    EquipajeId int NOT NULL AUTO_INCREMENT,
    BoletoId int NOT NULL,
    Codigo varchar(14) NOT NULL,
    PesoKg decimal(5,2) NOT NULL,
    Estado varchar(20) NOT NULL,
    CONSTRAINT PK_Equipaje_EquipajeId PRIMARY KEY (EquipajeId),
    CONSTRAINT FK_Equipaje_BoletoId FOREIGN KEY (BoletoId)
        REFERENCES Boleto (BoletoId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Equipaje_Codigo UNIQUE (Codigo),
    CONSTRAINT CK_Equipaje_PesoKg CHECK (PesoKg > 0 AND PesoKg <= 32),
    CONSTRAINT CK_Equipaje_Estado CHECK (Estado IN ('Registrado', 'Cargado', 'Entregado')),
    INDEX IX_Equipaje_BoletoId (BoletoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS TripulacionVuelo (
    VueloId int NOT NULL,
    EmpleadoId int NOT NULL,
    RolABordo varchar(30) NOT NULL,
    CONSTRAINT PK_TripulacionVuelo_VueloId_EmpleadoId PRIMARY KEY (VueloId, EmpleadoId),
    CONSTRAINT FK_TripulacionVuelo_VueloId FOREIGN KEY (VueloId)
        REFERENCES Vuelo (VueloId) ON DELETE NO ACTION,
    CONSTRAINT FK_TripulacionVuelo_EmpleadoId FOREIGN KEY (EmpleadoId)
        REFERENCES Empleado (EmpleadoId) ON DELETE NO ACTION,
    INDEX IX_TripulacionVuelo_EmpleadoId (EmpleadoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
