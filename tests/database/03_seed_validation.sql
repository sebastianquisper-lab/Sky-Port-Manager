-- Ejecutar en una base de pruebas con solo 01_schema.sql, 02_identity.sql y 03_seed.sql.
-- Todas las filas deben mostrar PASS; este archivo no modifica datos.
USE skyport_db;
SET @j04_validacion_zona = @@session.time_zone;
SET time_zone = '-05:00';

SELECT Prueba, Esperado, Actual,
       IF(Actual = Esperado, 'PASS', 'FAIL') AS Resultado
FROM (
    SELECT 'Aerolinea' AS Prueba, 4 AS Esperado, COUNT(*) AS Actual FROM Aerolinea
    UNION ALL SELECT 'Aeronave', 8, COUNT(*) FROM Aeronave
    UNION ALL SELECT 'Terminal', 2, COUNT(*) FROM Terminal
    UNION ALL SELECT 'Puerta', 10, COUNT(*) FROM Puerta
    UNION ALL SELECT 'Ruta', 10, COUNT(*) FROM Ruta
    UNION ALL SELECT 'Vuelo', 12, COUNT(*) FROM Vuelo
    UNION ALL SELECT 'Pasajero', 20, COUNT(*) FROM Pasajero
    UNION ALL SELECT 'Reserva', 25, COUNT(*) FROM Reserva
    UNION ALL SELECT 'Boleto', 15, COUNT(*) FROM Boleto
    UNION ALL SELECT 'CheckIn', 3, COUNT(*) FROM CheckIn
    UNION ALL SELECT 'Embarque', 2, COUNT(*) FROM Embarque
    UNION ALL SELECT 'Equipaje', 10, COUNT(*) FROM Equipaje
    UNION ALL SELECT 'Empleado', 8, COUNT(*) FROM Empleado
    UNION ALL SELECT 'TripulacionVuelo', 12, COUNT(*) FROM TripulacionVuelo
    UNION ALL SELECT 'Vuelos con tripulacion', 4, COUNT(DISTINCT VueloId) FROM TripulacionVuelo
    UNION ALL SELECT 'Estados de vuelo variados', 5, COUNT(DISTINCT Estado) FROM Vuelo
    UNION ALL SELECT 'Reservas confirmadas', 24, COUNT(*) FROM Reserva WHERE Estado = 'Confirmada'
    UNION ALL SELECT 'Reservas canceladas', 1, COUNT(*) FROM Reserva WHERE Estado = 'Cancelada'
    UNION ALL SELECT 'Capacidades fuera de rango', 0, COUNT(*) FROM Aeronave WHERE Capacidad NOT BETWEEN 120 AND 186
    UNION ALL SELECT 'Vuelos con aeronave no operativa', 0, COUNT(*)
        FROM Vuelo v JOIN Aeronave a ON a.AeronaveId = v.AeronaveId WHERE a.Estado <> 'Operativa'
    UNION ALL SELECT 'Vuelos fuera de los proximos 12 dias', 0, COUNT(*) FROM Vuelo
        WHERE FechaSalida < DATE_ADD(NOW(), INTERVAL 23 HOUR) OR FechaSalida > DATE_ADD(NOW(), INTERVAL 13 DAY)
    UNION ALL SELECT 'Duracion de vuelos incorrecta', 0, COUNT(*)
        FROM Vuelo v JOIN Ruta r ON r.RutaId = v.RutaId
        WHERE TIMESTAMPDIFF(MINUTE, v.FechaSalida, v.FechaLlegada) <> r.DuracionMinutos
    UNION ALL SELECT 'Asientos repetidos por vuelo', 0, COUNT(*) FROM (
        SELECT VueloId, Asiento FROM Reserva GROUP BY VueloId, Asiento HAVING COUNT(*) > 1
    ) asientos_repetidos
    UNION ALL SELECT 'Reservas activas repetidas de pasajero por vuelo', 0, COUNT(*) FROM (
        SELECT VueloId, PasajeroId FROM Reserva WHERE Estado = 'Confirmada'
        GROUP BY VueloId, PasajeroId HAVING COUNT(*) > 1
    ) pasajeros_repetidos
    UNION ALL SELECT 'Columnas generadas incoherentes', 0, COUNT(*) FROM Reserva
        WHERE (Estado = 'Confirmada' AND (NOT (AsientoActivo <=> Asiento) OR NOT (PasajeroActivoId <=> PasajeroId)))
           OR (Estado = 'Cancelada' AND (AsientoActivo IS NOT NULL OR PasajeroActivoId IS NOT NULL))
    UNION ALL SELECT 'Boletos incoherentes con reserva', 0, COUNT(*)
        FROM Boleto b JOIN Reserva r ON r.ReservaId = b.ReservaId
        WHERE (b.Estado = 'Cancelado' AND r.Estado <> 'Cancelada')
           OR (b.Estado <> 'Cancelado' AND r.Estado <> 'Confirmada')
    UNION ALL SELECT 'Check-in incoherentes', 0, COUNT(*)
        FROM CheckIn c JOIN Boleto b ON b.BoletoId = c.BoletoId
        JOIN Reserva r ON r.ReservaId = b.ReservaId JOIN Vuelo v ON v.VueloId = r.VueloId
        WHERE c.DocumentoVerificado <> 1 OR c.FechaHora >= v.FechaSalida
           OR b.Estado NOT IN ('CheckInRealizado', 'Abordado')
    UNION ALL SELECT 'Embarques incoherentes', 0, COUNT(*)
        FROM Embarque e JOIN Boleto b ON b.BoletoId = e.BoletoId
        JOIN Reserva r ON r.ReservaId = b.ReservaId JOIN Vuelo v ON v.VueloId = r.VueloId
        LEFT JOIN CheckIn c ON c.BoletoId = b.BoletoId
        WHERE b.Estado <> 'Abordado' OR c.CheckInId IS NULL
           OR e.FechaHora < c.FechaHora OR e.FechaHora >= v.FechaSalida
           OR NOT (e.PuertaId <=> v.PuertaId)
    UNION ALL SELECT 'Equipajes con peso invalido', 0, COUNT(*) FROM Equipaje WHERE PesoKg <= 0 OR PesoKg > 32
    UNION ALL SELECT 'Equipajes cargados sin check-in', 0, COUNT(*)
        FROM Equipaje e JOIN Boleto b ON b.BoletoId = e.BoletoId
        WHERE e.Estado = 'Cargado' AND b.Estado NOT IN ('CheckInRealizado', 'Abordado')
    UNION ALL SELECT 'Mas de tres equipajes por boleto', 0, COUNT(*) FROM (
        SELECT BoletoId FROM Equipaje GROUP BY BoletoId HAVING COUNT(*) > 3
    ) exceso_equipajes
    UNION ALL SELECT 'Roles de tripulacion distintos del cargo', 0, COUNT(*)
        FROM TripulacionVuelo t JOIN Empleado e ON e.EmpleadoId = t.EmpleadoId WHERE t.RolABordo <> e.Cargo
) comprobaciones;

SET time_zone = @j04_validacion_zona;
