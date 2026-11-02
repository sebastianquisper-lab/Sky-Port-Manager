-- Ejecutar solo sobre una base de pruebas recien creada con 01_schema.sql.
-- Usar mysql --force para continuar tras los cinco errores esperados.
-- Los registros de apoyo se revierten al final; no son datos de semilla.
USE skyport_db;
START TRANSACTION;

INSERT INTO Aerolinea (Nombre, CodigoIata, Pais)
VALUES ('J02 Pruebas', 'J2', 'Peru');
SET @aerolinea = LAST_INSERT_ID();

INSERT INTO Aeronave (AerolineaId, Matricula, Modelo, Capacidad, Estado)
VALUES (@aerolinea, 'J02-VALIDA', 'Prueba', 180, 'Operativa');
SET @aeronave = LAST_INSERT_ID();

INSERT INTO Ruta (CodigoOrigen, CiudadOrigen, CodigoDestino, CiudadDestino, DuracionMinutos)
VALUES ('LIM', 'Lima', 'CUZ', 'Cusco', 80);
SET @ruta = LAST_INSERT_ID();

INSERT INTO Vuelo (NumeroVuelo, RutaId, AeronaveId, FechaSalida, FechaLlegada, Estado)
VALUES ('J02001', @ruta, @aeronave, '2030-01-01 10:00:00', '2030-01-01 11:20:00', 'Programado');
SET @vuelo = LAST_INSERT_ID();

INSERT INTO Pasajero (TipoDocumento, NumeroDocumento, Nombres, Apellidos, FechaNacimiento, Nacionalidad)
VALUES ('DNI', 'J0200001', 'Prueba Uno', 'J02', '1990-01-01', 'Peruana');
SET @pasajero1 = LAST_INSERT_ID();
INSERT INTO Pasajero (TipoDocumento, NumeroDocumento, Nombres, Apellidos, FechaNacimiento, Nacionalidad)
VALUES ('DNI', 'J0200002', 'Prueba Dos', 'J02', '1991-01-01', 'Peruana');
SET @pasajero2 = LAST_INSERT_ID();

INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
VALUES ('J02BASE', @pasajero1, @vuelo, '1A', 'Confirmada');
SET @reserva = LAST_INSERT_ID();

INSERT INTO Terminal (Nombre) VALUES ('J02 Terminal');
SET @terminal = LAST_INSERT_ID();
INSERT INTO Puerta (TerminalId, Codigo) VALUES (@terminal, 'J02');
SET @puerta = LAST_INSERT_ID();
INSERT INTO Empleado (NumeroDocumento, Nombres, Apellidos, Cargo)
VALUES ('J02EMP', 'Prueba', 'J02', 'Piloto');
SET @empleado = LAST_INSERT_ID();
INSERT INTO TripulacionVuelo (VueloId, EmpleadoId, RolABordo)
VALUES (@vuelo, @empleado, 'Piloto');
INSERT INTO Boleto (ReservaId, CodigoBoleto, Estado)
VALUES (@reserva, 'J02BOLETO', 'Emitido');
SET @boleto = LAST_INSERT_ID();
INSERT INTO CheckIn (BoletoId, FechaHora, DocumentoVerificado)
VALUES (@boleto, '2030-01-01 09:00:00', 1);
INSERT INTO Embarque (BoletoId, FechaHora)
VALUES (@boleto, '2030-01-01 09:30:00');
INSERT INTO Equipaje (BoletoId, Codigo, PesoKg, Estado)
VALUES (@boleto, 'J02EQUIPAJE', 32.00, 'Registrado');

-- 1. Error 3819: CK_Aeronave_Capacidad (minimo 1).
INSERT INTO Aeronave (AerolineaId, Matricula, Modelo, Capacidad, Estado)
VALUES (@aerolinea, 'J02-CAP0', 'Prueba', 0, 'Operativa');

-- 2. Error 1452: FK_Aeronave_AerolineaId (aerolinea inexistente).
INSERT INTO Aeronave (AerolineaId, Matricula, Modelo, Capacidad, Estado)
VALUES (-1, 'J02-SIN-FK', 'Prueba', 180, 'Operativa');

-- 3. Error 3819: CK_Reserva_Estado (estado fuera de la lista).
INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
VALUES ('J02ESTADO', @pasajero2, @vuelo, '1B', 'Pendiente');

-- 4. Error 1062: UQ_Reserva_VueloId_AsientoActivo.
-- Otro pasajero intenta confirmar el mismo asiento del mismo vuelo.
INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
VALUES ('J02ASIENTO', @pasajero2, @vuelo, '1A', 'Confirmada');

-- 5. Error 1062: UQ_Reserva_VueloId_PasajeroActivoId.
-- El mismo pasajero intenta confirmar otro asiento del mismo vuelo.
INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
VALUES ('J02PASAJE', @pasajero1, @vuelo, '1B', 'Confirmada');

SELECT IF(
    (SELECT COUNT(*) FROM Aeronave) = 1
    AND (SELECT COUNT(*) FROM Reserva) = 1,
    'PASS: cinco INSERT rechazados', 'FAIL: filas inesperadas'
) AS Resultado;

UPDATE Reserva SET Estado = 'Cancelada' WHERE ReservaId = @reserva;
SELECT IF(AsientoActivo IS NULL AND PasajeroActivoId IS NULL,
    'PASS: columnas generadas NULL al cancelar', 'FAIL: columnas generadas') AS Resultado
FROM Reserva WHERE ReservaId = @reserva;

-- Ahora el segundo pasajero SI puede ocupar 1A.
INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
VALUES ('J02REUSO', @pasajero2, @vuelo, '1A', 'Confirmada');

-- El primer pasajero tambien puede volver a reservar en el mismo vuelo.
INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
VALUES ('J02VUELVE', @pasajero1, @vuelo, '1B', 'Confirmada');

SELECT IF(
    (SELECT COUNT(*) FROM Reserva WHERE VueloId = @vuelo AND Estado = 'Confirmada') = 2
    AND EXISTS (SELECT 1 FROM Reserva WHERE Codigo = 'J02REUSO' AND AsientoActivo = '1A' AND PasajeroActivoId = @pasajero2)
    AND EXISTS (SELECT 1 FROM Reserva WHERE Codigo = 'J02VUELVE' AND AsientoActivo = '1B' AND PasajeroActivoId = @pasajero1),
    'PASS: asiento y pasajero reutilizables tras cancelar', 'FAIL: reutilizacion'
) AS Resultado;

SELECT IF(
    (SELECT Activa FROM Aerolinea WHERE AerolineaId = @aerolinea) = 1
    AND (SELECT Disponible FROM Puerta WHERE PuertaId = @puerta) = 1
    AND (SELECT Activo FROM Empleado WHERE EmpleadoId = @empleado) = 1
    AND (SELECT FechaReserva IS NOT NULL FROM Reserva WHERE ReservaId = @reserva)
    AND (SELECT FechaEmision IS NOT NULL FROM Boleto WHERE BoletoId = @boleto),
    'PASS: cinco defaults solicitados', 'FAIL: defaults'
) AS Resultado;

ROLLBACK;
SELECT IF(
    (SELECT COUNT(*) FROM Aerolinea) = 0
    AND (SELECT COUNT(*) FROM Aeronave) = 0
    AND (SELECT COUNT(*) FROM Ruta) = 0
    AND (SELECT COUNT(*) FROM Vuelo) = 0
    AND (SELECT COUNT(*) FROM Pasajero) = 0
    AND (SELECT COUNT(*) FROM Reserva) = 0
    AND (SELECT COUNT(*) FROM Terminal) = 0
    AND (SELECT COUNT(*) FROM Puerta) = 0
    AND (SELECT COUNT(*) FROM Empleado) = 0
    AND (SELECT COUNT(*) FROM TripulacionVuelo) = 0
    AND (SELECT COUNT(*) FROM Boleto) = 0
    AND (SELECT COUNT(*) FROM CheckIn) = 0
    AND (SELECT COUNT(*) FROM Embarque) = 0
    AND (SELECT COUNT(*) FROM Equipaje) = 0,
    'PASS: sin datos de prueba persistidos', 'FAIL: rollback'
) AS Resultado;
