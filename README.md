# SkyPort Manager
Sistema de gestión aeroportuaria desarrollado con .NET 10, Entity Framework Core y PostgreSQL.

## Frontend

La interfaz está en `Sky.API/wwwroot` y se sirve desde la misma aplicación .NET. Usa HTML, CSS y JavaScript sin dependencias npm ni un proceso de compilación adicional.

Incluye inicio de sesión, resumen de operaciones del día, consulta de vuelos, búsqueda por código/aerolínea/aeropuerto, filtros por fecha y estado, detalle de vuelo y ocupación. Los horarios se muestran en la zona local del navegador. El filtro de fecha usa el día local de salida; **Limpiar filtros** permite consultar todas las fechas.

### Ejecutar

1. Instalar el SDK de .NET 10 y preparar la base PostgreSQL del proyecto (incluidas las vistas `v_flight_board` y `v_flight_occupancy`). Este repositorio no incluye un script de creación de la base.
2. Configurar `ConnectionStrings:SkyportDb` y `Jwt:Key` mediante user-secrets del proyecto `Sky.API` o variables de entorno (`ConnectionStrings__SkyportDb`, `Jwt__Key`). La clave JWT debe tener al menos 32 bytes para HS256. No guardar credenciales en el repositorio.
3. Ejecutar desde la raíz:

   ```powershell
   dotnet run --project Sky.API --launch-profile https
   ```

4. Abrir `https://localhost:7248`. Si falta el certificado de desarrollo, ejecutar `dotnet dev-certs https --trust`.

Usar una cuenta activa existente en la base de datos para el login. **Ver demostración** permite explorar la interfaz con datos ficticios, sin consultar la API ni modificar registros. La aplicación .NET debe estar iniciada y contar con la configuración de arranque incluso para acceder a esta demostración.

El token se conserva únicamente en memoria y se elimina al salir o recargar la página. La interfaz vuelve al login cuando vence la sesión o la API responde 401. Las consultas usan `/api/auth/login`, `/api/flight-board` y `/api/flight-occupancy` en el mismo origen, sin configuración CORS adicional. La primera versión permite consultar vuelos; no incluye creación ni edición.

### Verificación del frontend

```powershell
dotnet build SkyPort.sln
node --check Sky.API/wwwroot/app.js
node scripts/frontend-smoke.mjs
```

La prueba de navegador requiere Node.js 22+ y Microsoft Edge en Windows (se puede indicar otra ruta con `EDGE_PATH`). Inicia un servidor temporal con respuestas simuladas y comprueba demostración, filtros, detalle, adaptación móvil, login, envío del JWT, escape de datos, errores de actualización y cierre de sesión. No requiere credenciales ni sustituye una prueba contra la base real.

## Integrantes
- Quispe Riquelme Sebastian (líder y backend)
- Johan David Vallejos Vilcape (base de datos y Entity Framework)
- Kalhel Eithan Gómez Avalos (interfaz de usuario)
- Victor Manuel Huacacha Ccahua (pruebas y documentación)
