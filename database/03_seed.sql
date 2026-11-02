USE skyport_db;

-- J04: datos ficticios de demostracion. Ejecutar despues de 01_schema.sql y 02_identity.sql.
-- Los documentos y matriculas FIC-J04-* no identifican personas ni aeronaves reales.
-- Todas las referencias se resuelven por claves de negocio, sin asumir IDs consecutivos.
-- Cada fila se inserta solo si falta. No se ignoran errores de CHECK ni de claves foraneas.
-- Los numeros de vuelo se reservan para esta semilla: el guard por NumeroVuelo evita
-- duplicados aunque NOW() cambie entre ejecuciones. No se reprograman vuelos existentes.
-- Los estados y eventos futuros son escenarios sinteticos para probar las pantallas.

SET @j04_zona_anterior = @@session.time_zone;
SET time_zone = '-05:00';
START TRANSACTION;

-- 4 aerolineas
INSERT INTO Aerolinea (Nombre, CodigoIata, Pais)
SELECT semilla.Nombre, semilla.CodigoIata, semilla.Pais
FROM (
    SELECT 'LATAM' AS Nombre, 'LA' AS CodigoIata, 'Peru' AS Pais FROM DUAL
    UNION ALL SELECT 'Sky Airline', 'H2', 'Chile' FROM DUAL
    UNION ALL SELECT 'JetSMART', 'JA', 'Chile' FROM DUAL
    UNION ALL SELECT 'Avianca', 'AV', 'Colombia' FROM DUAL
) AS semilla
WHERE NOT EXISTS (
    SELECT 1 FROM Aerolinea AS existente WHERE existente.CodigoIata = semilla.CodigoIata OR existente.Nombre = semilla.Nombre
);

-- 8 aeronaves (capacidades 120 a 186)
INSERT INTO Aeronave (AerolineaId, Matricula, Modelo, Capacidad, Estado)
SELECT a.AerolineaId, semilla.Matricula, semilla.Modelo, semilla.Capacidad, semilla.Estado
FROM (
    SELECT 'LA' AS CodigoIata, 'FIC-J04-LA01' AS Matricula, 'Airbus A320' AS Modelo, 180 AS Capacidad, 'Operativa' AS Estado FROM DUAL
    UNION ALL SELECT 'LA', 'FIC-J04-LA02', 'Airbus A320neo', 186, 'Operativa' FROM DUAL
    UNION ALL SELECT 'H2', 'FIC-J04-H201', 'Airbus A319', 150, 'Operativa' FROM DUAL
    UNION ALL SELECT 'H2', 'FIC-J04-H202', 'Airbus A319', 120, 'Mantenimiento' FROM DUAL
    UNION ALL SELECT 'JA', 'FIC-J04-JA01', 'Airbus A320neo', 186, 'Operativa' FROM DUAL
    UNION ALL SELECT 'JA', 'FIC-J04-JA02', 'Airbus A320', 180, 'Operativa' FROM DUAL
    UNION ALL SELECT 'AV', 'FIC-J04-AV01', 'Airbus A320', 180, 'Operativa' FROM DUAL
    UNION ALL SELECT 'AV', 'FIC-J04-AV02', 'Airbus A319', 150, 'FueraDeServicio' FROM DUAL
) AS semilla
JOIN Aerolinea AS a ON a.CodigoIata = semilla.CodigoIata
WHERE NOT EXISTS (
    SELECT 1 FROM Aeronave AS existente WHERE existente.Matricula = semilla.Matricula
);

-- 2 terminales
INSERT INTO Terminal (Nombre)
SELECT semilla.Nombre
FROM (
    SELECT 'Nacional' AS Nombre FROM DUAL
    UNION ALL SELECT 'Internacional' FROM DUAL
) AS semilla
WHERE NOT EXISTS (
    SELECT 1 FROM Terminal AS existente WHERE existente.Nombre = semilla.Nombre
);

-- 10 puertas
INSERT INTO Puerta (TerminalId, Codigo)
SELECT t.TerminalId, semilla.Codigo
FROM (
    SELECT 'Nacional' AS Terminal, 'N01' AS Codigo FROM DUAL
    UNION ALL SELECT 'Nacional', 'N02' FROM DUAL
    UNION ALL SELECT 'Nacional', 'N03' FROM DUAL
    UNION ALL SELECT 'Nacional', 'N04' FROM DUAL
    UNION ALL SELECT 'Nacional', 'N05' FROM DUAL
    UNION ALL SELECT 'Internacional', 'I01' FROM DUAL
    UNION ALL SELECT 'Internacional', 'I02' FROM DUAL
    UNION ALL SELECT 'Internacional', 'I03' FROM DUAL
    UNION ALL SELECT 'Internacional', 'I04' FROM DUAL
    UNION ALL SELECT 'Internacional', 'I05' FROM DUAL
) AS semilla
JOIN Terminal AS t ON t.Nombre = semilla.Terminal
WHERE NOT EXISTS (
    SELECT 1 FROM Puerta AS existente WHERE existente.TerminalId = t.TerminalId AND existente.Codigo = semilla.Codigo
);

-- 10 rutas
INSERT INTO Ruta (CodigoOrigen, CiudadOrigen, CodigoDestino, CiudadDestino, DuracionMinutos)
SELECT semilla.CodigoOrigen, semilla.CiudadOrigen, semilla.CodigoDestino, semilla.CiudadDestino, semilla.DuracionMinutos
FROM (
    SELECT 'LIM' AS CodigoOrigen, 'Lima' AS CiudadOrigen, 'CUZ' AS CodigoDestino, 'Cusco' AS CiudadDestino, 80 AS DuracionMinutos FROM DUAL
    UNION ALL SELECT 'CUZ', 'Cusco', 'LIM', 'Lima', 85 FROM DUAL
    UNION ALL SELECT 'LIM', 'Lima', 'AQP', 'Arequipa', 95 FROM DUAL
    UNION ALL SELECT 'AQP', 'Arequipa', 'LIM', 'Lima', 100 FROM DUAL
    UNION ALL SELECT 'LIM', 'Lima', 'PIU', 'Piura', 100 FROM DUAL
    UNION ALL SELECT 'PIU', 'Piura', 'LIM', 'Lima', 105 FROM DUAL
    UNION ALL SELECT 'LIM', 'Lima', 'IQT', 'Iquitos', 110 FROM DUAL
    UNION ALL SELECT 'IQT', 'Iquitos', 'LIM', 'Lima', 115 FROM DUAL
    UNION ALL SELECT 'LIM', 'Lima', 'SCL', 'Santiago', 210 FROM DUAL
    UNION ALL SELECT 'BOG', 'Bogota', 'LIM', 'Lima', 190 FROM DUAL
) AS semilla
WHERE NOT EXISTS (
    SELECT 1 FROM Ruta AS existente WHERE existente.CodigoOrigen = semilla.CodigoOrigen AND existente.CodigoDestino = semilla.CodigoDestino
);

-- 12 vuelos, entre 1 y 12 dias desde la primera carga
INSERT INTO Vuelo (NumeroVuelo, RutaId, AeronaveId, PuertaId, FechaSalida, FechaLlegada, Estado)
SELECT semilla.NumeroVuelo, r.RutaId, a.AeronaveId, p.PuertaId,
       DATE_ADD(NOW(), INTERVAL semilla.Dias DAY),
       DATE_ADD(DATE_ADD(NOW(), INTERVAL semilla.Dias DAY), INTERVAL r.DuracionMinutos MINUTE),
       semilla.Estado
FROM (
    SELECT 'LA9001' AS NumeroVuelo, 'LIM' AS Origen, 'CUZ' AS Destino, 'FIC-J04-LA01' AS Matricula, 'Nacional' AS Terminal, 'N01' AS Puerta, 1 AS Dias, 'Embarcando' AS Estado FROM DUAL
    UNION ALL SELECT 'LA9002', 'CUZ', 'LIM', 'FIC-J04-LA02', 'Nacional', 'N02', 2, 'Programado' FROM DUAL
    UNION ALL SELECT 'H29001', 'LIM', 'AQP', 'FIC-J04-H201', 'Nacional', 'N03', 3, 'Retrasado' FROM DUAL
    UNION ALL SELECT 'H29002', 'AQP', 'LIM', 'FIC-J04-H201', 'Nacional', 'N04', 4, 'Programado' FROM DUAL
    UNION ALL SELECT 'JA9001', 'LIM', 'PIU', 'FIC-J04-JA01', 'Nacional', 'N05', 5, 'Programado' FROM DUAL
    UNION ALL SELECT 'JA9002', 'PIU', 'LIM', 'FIC-J04-JA02', 'Nacional', 'N01', 6, 'Cancelado' FROM DUAL
    UNION ALL SELECT 'AV9001', 'LIM', 'IQT', 'FIC-J04-AV01', 'Nacional', 'N02', 7, 'Programado' FROM DUAL
    UNION ALL SELECT 'AV9002', 'IQT', 'LIM', 'FIC-J04-AV01', 'Nacional', 'N03', 8, 'Programado' FROM DUAL
    UNION ALL SELECT 'LA9003', 'LIM', 'SCL', 'FIC-J04-LA01', 'Internacional', 'I01', 9, 'Retrasado' FROM DUAL
    UNION ALL SELECT 'H29003', 'BOG', 'LIM', 'FIC-J04-H201', 'Internacional', 'I02', 10, 'Programado' FROM DUAL
    UNION ALL SELECT 'JA9003', 'LIM', 'SCL', 'FIC-J04-JA01', 'Internacional', 'I03', 11, 'Programado' FROM DUAL
    UNION ALL SELECT 'AV9003', 'BOG', 'LIM', 'FIC-J04-AV01', 'Internacional', 'I04', 12, 'Despegado' FROM DUAL
) AS semilla
JOIN Ruta AS r ON r.CodigoOrigen = semilla.Origen AND r.CodigoDestino = semilla.Destino
JOIN Aeronave AS a ON a.Matricula = semilla.Matricula
JOIN Terminal AS t ON t.Nombre = semilla.Terminal
JOIN Puerta AS p ON p.TerminalId = t.TerminalId AND p.Codigo = semilla.Puerta
WHERE NOT EXISTS (
    SELECT 1 FROM Vuelo AS existente WHERE existente.NumeroVuelo = semilla.NumeroVuelo
);

-- 20 pasajeros ficticios; contactos en example.invalid
INSERT INTO Pasajero (TipoDocumento, NumeroDocumento, Nombres, Apellidos, FechaNacimiento, Nacionalidad, Email)
SELECT semilla.TipoDocumento, semilla.NumeroDocumento, semilla.Nombres, semilla.Apellidos,
       semilla.FechaNacimiento, semilla.Nacionalidad, semilla.Email
FROM (
    SELECT 'DNI' AS TipoDocumento, 'FIC-J04-001' AS NumeroDocumento, 'Alba' AS Nombres, 'Demo01' AS Apellidos, '1985-03-15' AS FechaNacimiento, 'Peruana' AS Nacionalidad, 'pasajero01@example.invalid' AS Email FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-002', 'Bruno', 'Demo02', '1986-03-15', 'Peruana', 'pasajero02@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-003', 'Celia', 'Demo03', '1987-03-15', 'Peruana', 'pasajero03@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-004', 'Dario', 'Demo04', '1988-03-15', 'Peruana', 'pasajero04@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-005', 'Elia', 'Demo05', '1989-03-15', 'Peruana', 'pasajero05@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-006', 'Fabio', 'Demo06', '1990-03-15', 'Peruana', 'pasajero06@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-007', 'Gala', 'Demo07', '1991-03-15', 'Peruana', 'pasajero07@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-008', 'Hugo', 'Demo08', '1992-03-15', 'Peruana', 'pasajero08@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-009', 'Iris', 'Demo09', '1993-03-15', 'Peruana', 'pasajero09@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-010', 'Joel', 'Demo10', '1994-03-15', 'Peruana', 'pasajero10@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-011', 'Kiara', 'Demo11', '1995-03-15', 'Peruana', 'pasajero11@example.invalid' FROM DUAL
    UNION ALL SELECT 'DNI', 'FIC-J04-012', 'Lino', 'Demo12', '1996-03-15', 'Peruana', 'pasajero12@example.invalid' FROM DUAL
    UNION ALL SELECT 'Pasaporte', 'FIC-J04-013', 'Mara', 'Demo13', '1997-03-15', 'Chilena', 'pasajero13@example.invalid' FROM DUAL
    UNION ALL SELECT 'Pasaporte', 'FIC-J04-014', 'Nilo', 'Demo14', '1998-03-15', 'Chilena', 'pasajero14@example.invalid' FROM DUAL
    UNION ALL SELECT 'Pasaporte', 'FIC-J04-015', 'Olga', 'Demo15', '1999-03-15', 'Chilena', 'pasajero15@example.invalid' FROM DUAL
    UNION ALL SELECT 'Pasaporte', 'FIC-J04-016', 'Pablo', 'Demo16', '2000-03-15', 'Chilena', 'pasajero16@example.invalid' FROM DUAL
    UNION ALL SELECT 'CE', 'FIC-J04-017', 'Quena', 'Demo17', '2001-03-15', 'Colombiana', 'pasajero17@example.invalid' FROM DUAL
    UNION ALL SELECT 'CE', 'FIC-J04-018', 'Rita', 'Demo18', '2002-03-15', 'Colombiana', 'pasajero18@example.invalid' FROM DUAL
    UNION ALL SELECT 'CE', 'FIC-J04-019', 'Saul', 'Demo19', '2003-03-15', 'Colombiana', 'pasajero19@example.invalid' FROM DUAL
    UNION ALL SELECT 'CE', 'FIC-J04-020', 'Tania', 'Demo20', '2004-03-15', 'Colombiana', 'pasajero20@example.invalid' FROM DUAL
) AS semilla
WHERE NOT EXISTS (
    SELECT 1 FROM Pasajero AS existente WHERE existente.TipoDocumento = semilla.TipoDocumento AND existente.NumeroDocumento = semilla.NumeroDocumento
);

-- 25 reservas (24 confirmadas y 1 cancelada)
INSERT INTO Reserva (Codigo, PasajeroId, VueloId, Asiento, Estado)
SELECT semilla.Codigo, p.PasajeroId, v.VueloId, semilla.Asiento, semilla.Estado
FROM (
    SELECT 'J04R001' AS Codigo, 'DNI' AS TipoDocumento, 'FIC-J04-001' AS NumeroDocumento, 'LA9001' AS NumeroVuelo, '1A' AS Asiento, 'Confirmada' AS Estado FROM DUAL
    UNION ALL SELECT 'J04R002', 'DNI', 'FIC-J04-002', 'LA9001', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R003', 'DNI', 'FIC-J04-003', 'LA9001', '1C', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R004', 'DNI', 'FIC-J04-004', 'LA9001', '1D', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R005', 'DNI', 'FIC-J04-005', 'LA9002', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R006', 'DNI', 'FIC-J04-006', 'LA9002', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R007', 'DNI', 'FIC-J04-007', 'LA9002', '1C', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R008', 'DNI', 'FIC-J04-008', 'H29001', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R009', 'DNI', 'FIC-J04-009', 'H29001', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R010', 'DNI', 'FIC-J04-010', 'H29001', '1C', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R011', 'DNI', 'FIC-J04-011', 'H29002', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R012', 'DNI', 'FIC-J04-012', 'H29002', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R013', 'Pasaporte', 'FIC-J04-013', 'JA9001', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R014', 'Pasaporte', 'FIC-J04-014', 'JA9001', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R015', 'Pasaporte', 'FIC-J04-015', 'JA9002', '1A', 'Cancelada' FROM DUAL
    UNION ALL SELECT 'J04R016', 'Pasaporte', 'FIC-J04-016', 'AV9001', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R017', 'CE', 'FIC-J04-017', 'AV9001', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R018', 'CE', 'FIC-J04-018', 'AV9002', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R019', 'CE', 'FIC-J04-019', 'AV9002', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R020', 'CE', 'FIC-J04-020', 'LA9003', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R021', 'DNI', 'FIC-J04-001', 'LA9003', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R022', 'DNI', 'FIC-J04-002', 'H29003', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R023', 'DNI', 'FIC-J04-003', 'JA9003', '1A', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R024', 'DNI', 'FIC-J04-004', 'JA9003', '1B', 'Confirmada' FROM DUAL
    UNION ALL SELECT 'J04R025', 'DNI', 'FIC-J04-005', 'JA9003', '1C', 'Confirmada' FROM DUAL
) AS semilla
JOIN Pasajero AS p ON p.TipoDocumento = semilla.TipoDocumento AND p.NumeroDocumento = semilla.NumeroDocumento
JOIN Vuelo AS v ON v.NumeroVuelo = semilla.NumeroVuelo
WHERE NOT EXISTS (
    SELECT 1 FROM Reserva AS existente WHERE existente.Codigo = semilla.Codigo
);

-- 15 boletos coherentes con las reservas
INSERT INTO Boleto (ReservaId, CodigoBoleto, Estado)
SELECT r.ReservaId, semilla.CodigoBoleto, semilla.Estado
FROM (
    SELECT 'J04R001' AS CodigoReserva, 'SKP-J040000001' AS CodigoBoleto, 'Abordado' AS Estado FROM DUAL
    UNION ALL SELECT 'J04R002', 'SKP-J040000002', 'Abordado' FROM DUAL
    UNION ALL SELECT 'J04R003', 'SKP-J040000003', 'CheckInRealizado' FROM DUAL
    UNION ALL SELECT 'J04R004', 'SKP-J040000004', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R005', 'SKP-J040000005', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R006', 'SKP-J040000006', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R007', 'SKP-J040000007', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R008', 'SKP-J040000008', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R009', 'SKP-J040000009', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R010', 'SKP-J040000010', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R011', 'SKP-J040000011', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R012', 'SKP-J040000012', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R013', 'SKP-J040000013', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R014', 'SKP-J040000014', 'Emitido' FROM DUAL
    UNION ALL SELECT 'J04R015', 'SKP-J040000015', 'Cancelado' FROM DUAL
) AS semilla
JOIN Reserva AS r ON r.Codigo = semilla.CodigoReserva
WHERE NOT EXISTS (
    SELECT 1 FROM Boleto AS existente WHERE existente.CodigoBoleto = semilla.CodigoBoleto OR existente.ReservaId = r.ReservaId
);

-- 8 empleados
INSERT INTO Empleado (NumeroDocumento, Nombres, Apellidos, Cargo, Email)
SELECT semilla.NumeroDocumento, semilla.Nombres, semilla.Apellidos, semilla.Cargo, semilla.Email
FROM (
    SELECT 'FIC-J04-E01' AS NumeroDocumento, 'Ariel' AS Nombres, 'EquipoDemo01' AS Apellidos, 'Piloto' AS Cargo, 'empleado1@example.invalid' AS Email FROM DUAL
    UNION ALL SELECT 'FIC-J04-E02', 'Belen', 'EquipoDemo02', 'Piloto', 'empleado2@example.invalid' FROM DUAL
    UNION ALL SELECT 'FIC-J04-E03', 'Ciro', 'EquipoDemo03', 'Copiloto', 'empleado3@example.invalid' FROM DUAL
    UNION ALL SELECT 'FIC-J04-E04', 'Dana', 'EquipoDemo04', 'Copiloto', 'empleado4@example.invalid' FROM DUAL
    UNION ALL SELECT 'FIC-J04-E05', 'Enzo', 'EquipoDemo05', 'TripulanteCabina', 'empleado5@example.invalid' FROM DUAL
    UNION ALL SELECT 'FIC-J04-E06', 'Fara', 'EquipoDemo06', 'TripulanteCabina', 'empleado6@example.invalid' FROM DUAL
    UNION ALL SELECT 'FIC-J04-E07', 'Gino', 'EquipoDemo07', 'PersonalTierra', 'empleado7@example.invalid' FROM DUAL
    UNION ALL SELECT 'FIC-J04-E08', 'Hana', 'EquipoDemo08', 'PersonalTierra', 'empleado8@example.invalid' FROM DUAL
) AS semilla
WHERE NOT EXISTS (
    SELECT 1 FROM Empleado AS existente WHERE existente.NumeroDocumento = semilla.NumeroDocumento
);

-- 3 check-in de los boletos 1, 2 y 3
INSERT INTO CheckIn (BoletoId, EmpleadoId, FechaHora, DocumentoVerificado)
SELECT b.BoletoId, e.EmpleadoId, DATE_SUB(v.FechaSalida, INTERVAL 120 MINUTE), 1
FROM (
    SELECT 'SKP-J040000001' AS CodigoBoleto, 'FIC-J04-E07' AS DocumentoEmpleado FROM DUAL
    UNION ALL SELECT 'SKP-J040000002', 'FIC-J04-E08' FROM DUAL
    UNION ALL SELECT 'SKP-J040000003', 'FIC-J04-E07' FROM DUAL
) AS semilla
JOIN Boleto AS b ON b.CodigoBoleto = semilla.CodigoBoleto
JOIN Reserva AS r ON r.ReservaId = b.ReservaId
JOIN Vuelo AS v ON v.VueloId = r.VueloId
JOIN Empleado AS e ON e.NumeroDocumento = semilla.DocumentoEmpleado
WHERE NOT EXISTS (
    SELECT 1 FROM CheckIn AS existente WHERE existente.BoletoId = b.BoletoId
);

-- 2 embarques de los boletos 1 y 2
INSERT INTO Embarque (BoletoId, PuertaId, FechaHora)
SELECT b.BoletoId, v.PuertaId, DATE_SUB(v.FechaSalida, INTERVAL semilla.MinutosAntes MINUTE)
FROM (
    SELECT 'SKP-J040000001' AS CodigoBoleto, 45 AS MinutosAntes FROM DUAL
    UNION ALL SELECT 'SKP-J040000002', 40 FROM DUAL
) AS semilla
JOIN Boleto AS b ON b.CodigoBoleto = semilla.CodigoBoleto
JOIN Reserva AS r ON r.ReservaId = b.ReservaId
JOIN Vuelo AS v ON v.VueloId = r.VueloId
WHERE NOT EXISTS (
    SELECT 1 FROM Embarque AS existente WHERE existente.BoletoId = b.BoletoId
);

-- 10 equipajes, maximo 2 por boleto
INSERT INTO Equipaje (BoletoId, Codigo, PesoKg, Estado)
SELECT b.BoletoId, semilla.Codigo, semilla.PesoKg, semilla.Estado
FROM (
    SELECT 'SKP-J040000001' AS CodigoBoleto, 'EQ-J0400001' AS Codigo, 12.5 AS PesoKg, 'Cargado' AS Estado FROM DUAL
    UNION ALL SELECT 'SKP-J040000001', 'EQ-J0400002', 18.2, 'Cargado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000002', 'EQ-J0400003', 23, 'Cargado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000002', 'EQ-J0400004', 15.75, 'Cargado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000003', 'EQ-J0400005', 32, 'Cargado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000003', 'EQ-J0400006', 8.4, 'Cargado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000004', 'EQ-J0400007', 10.1, 'Registrado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000005', 'EQ-J0400008', 19.6, 'Registrado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000006', 'EQ-J0400009', 21.3, 'Registrado' FROM DUAL
    UNION ALL SELECT 'SKP-J040000007', 'EQ-J0400010', 14.8, 'Registrado' FROM DUAL
) AS semilla
JOIN Boleto AS b ON b.CodigoBoleto = semilla.CodigoBoleto
WHERE NOT EXISTS (
    SELECT 1 FROM Equipaje AS existente WHERE existente.Codigo = semilla.Codigo
);

-- 12 asignaciones de tripulacion para 4 vuelos
INSERT INTO TripulacionVuelo (VueloId, EmpleadoId, RolABordo)
SELECT v.VueloId, e.EmpleadoId, semilla.RolABordo
FROM (
    SELECT 'LA9001' AS NumeroVuelo, 'FIC-J04-E01' AS DocumentoEmpleado, 'Piloto' AS RolABordo FROM DUAL
    UNION ALL SELECT 'LA9001', 'FIC-J04-E03', 'Copiloto' FROM DUAL
    UNION ALL SELECT 'LA9001', 'FIC-J04-E05', 'TripulanteCabina' FROM DUAL
    UNION ALL SELECT 'LA9002', 'FIC-J04-E02', 'Piloto' FROM DUAL
    UNION ALL SELECT 'LA9002', 'FIC-J04-E04', 'Copiloto' FROM DUAL
    UNION ALL SELECT 'LA9002', 'FIC-J04-E06', 'TripulanteCabina' FROM DUAL
    UNION ALL SELECT 'H29001', 'FIC-J04-E01', 'Piloto' FROM DUAL
    UNION ALL SELECT 'H29001', 'FIC-J04-E03', 'Copiloto' FROM DUAL
    UNION ALL SELECT 'H29001', 'FIC-J04-E05', 'TripulanteCabina' FROM DUAL
    UNION ALL SELECT 'H29002', 'FIC-J04-E02', 'Piloto' FROM DUAL
    UNION ALL SELECT 'H29002', 'FIC-J04-E04', 'Copiloto' FROM DUAL
    UNION ALL SELECT 'H29002', 'FIC-J04-E06', 'TripulanteCabina' FROM DUAL
) AS semilla
JOIN Vuelo AS v ON v.NumeroVuelo = semilla.NumeroVuelo
JOIN Empleado AS e ON e.NumeroDocumento = semilla.DocumentoEmpleado
WHERE NOT EXISTS (
    SELECT 1 FROM TripulacionVuelo AS existente WHERE existente.VueloId = v.VueloId AND existente.EmpleadoId = e.EmpleadoId
);

COMMIT;
SET time_zone = @j04_zona_anterior;
