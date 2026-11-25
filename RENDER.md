# Publicar SkyPort Manager en Render

La solución .NET conserva sus cuatro proyectos. Solo se añaden archivos de despliegue: Docker compila la API con sus dependencias y la ejecuta en Linux; el frontend permanece en su propio repositorio.

## 1. Crear la API y PostgreSQL

En [Render](https://dashboard.render.com/), seleccionar **New → Blueprint**, conectar `sebastianquisper-lab/Sky-Port-Manager`, elegir la rama `main` y usar `render.yaml` en la raíz.

El Blueprint declara dos recursos en Oregon:

| Recurso | Función |
| --- | --- |
| `skyport-api` | Web Service con Docker y .NET 10 |
| `skyport-db` | PostgreSQL, inicialmente vacío |

Revisar los planes antes de confirmar. El archivo selecciona `free`; Render limita las bases gratuitas a una por workspace y expiran a los 30 días. Los servicios gratuitos pueden tardar en despertar tras un período de inactividad. [Límites oficiales](https://render.com/docs/free).

Render genera `Jwt__Key` y entrega las propiedades de PostgreSQL al contenedor. `deploy/start-api.sh` convierte esos campos a la cadena de conexión de Npgsql y configura la escucha en `0.0.0.0:$PORT`. La cadena no se imprime ni se guarda en Git.

No usar directamente la URL `postgresql://...` como `ConnectionStrings__SkyportDb`: Npgsql espera formato ADO.NET. El Blueprint ya hace la conversión mediante los campos `PGHOST`, `PGPORT`, `PGDATABASE`, `PGUSER` y `PGPASSWORD`.

La terminación HTTPS ocurre en Render. `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` permite que ASP.NET Core reconozca ese HTTPS detrás del proxy sin cambiar `Program.cs`. Esta configuración está destinada al servicio detrás del balanceador de Render.

No configurar un health check HTTP de la API apuntando a `/`: no existe una página de inicio. Tampoco apuntarlo a una ruta protegida, que responderá 401. Sin un health check HTTP explícito, se utiliza la comprobación TCP; esto verifica que el proceso escucha, no que la base ya esté importada.

## 2. Importar la base original (necesario para datos reales)

**Este repositorio no contiene la base, scripts SQL ni migraciones.** Los archivos de entidades y `DbContext` describen el mapeo, pero no reemplazan el esquema original, sus vistas, funciones, triggers y datos iniciales. No ejecutar `EnsureCreated` ni crear una migración improvisada para sustituirlos.

Pedir al integrante responsable un respaldo PostgreSQL o exportar la base local con `pg_dump`. Debe incluir esquema, vistas (especialmente `v_flight_board` y `v_flight_occupancy`), funciones, triggers, catálogos y una cuenta activa de prueba. Usar datos de demostración apropiados para la entrega.

Ejemplo en PowerShell, sustituyendo los valores locales por los del equipo; los programas solicitarán la contraseña cuando sea necesario:

```powershell
New-Item -ItemType Directory -Force artifacts | Out-Null
pg_dump --host localhost --port 5432 --username postgres --dbname skyport --format custom --no-owner --no-acl --file artifacts/skyport.dump
```

Después, copiar la **External Database URL** desde el panel de `skyport-db` a una variable temporal local, sin subirla al repositorio:

```powershell
$env:RENDER_DATABASE_URL = 'REEMPLAZAR_CON_LA_EXTERNAL_DATABASE_URL_DE_RENDER'
pg_restore --no-owner --no-acl --exit-on-error --dbname $env:RENDER_DATABASE_URL artifacts/skyport.dump
Remove-Item Env:RENDER_DATABASE_URL
```

Usar herramientas PostgreSQL de versión compatible con la base de origen y destino. Restaurar una sola vez sobre la base nueva; los comandos no borran tablas existentes y una segunda restauración puede producir conflictos. No versionar el respaldo ni credenciales: `artifacts/` ya está ignorado.

Si el respaldo es SQL de texto, importar con `psql --set ON_ERROR_STOP=on --dbname $env:RENDER_DATABASE_URL --file RUTA_DEL_ARCHIVO.sql`, en lugar de `pg_restore`.

Si la base ya está alojada fuera de Render, no crear otra vacía: usar un Web Service Docker manual y configurar `ConnectionStrings__SkyportDb` con la conexión existente en formato `Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require`, junto con `Jwt__Key` (mínimo 32 caracteres aleatorios). No usar el Blueprint de creación de base en ese caso.

## 3. Conectar el frontend

Esperar a que la API esté desplegada y copiar su URL HTTPS desde el panel de Render; puede ser distinta de `https://skyport-api.onrender.com`.

Crear un segundo **Blueprint** desde [johanvallejos-cyber/Sky-Port-Frontend](https://github.com/johanvallejos-cyber/Sky-Port-Frontend), rama `main`.

Cuando solicite `API_PROXY_TARGET`, introducir la URL pública HTTPS de la API, **sin `/api` al final**. El frontend se ejecuta como Web Service Node y reenvía `/api/*` al backend con un proxy de producción. No requiere cambiar CORS ni servir la interfaz desde .NET.

En la [guía del frontend](https://github.com/johanvallejos-cyber/Sky-Port-Frontend/blob/main/RENDER.md) están las opciones manuales equivalentes.

## 4. Comprobar la entrega

1. Abrir la URL pública del frontend y probar **Ver demostración**: no necesita base de datos.
2. Salir de la demostración e iniciar sesión con la cuenta activa importada.
3. Comprobar vuelos, filtros y detalles. Si no hay salidas del día, pulsar **Limpiar filtros**.
4. Entregar al profesor la URL del frontend para usar la aplicación y los enlaces de ambos repositorios para revisar el código.

Si el login da 500, revisar logs de la API y comprobar que la base y `app_user` fueron importadas. Si da 401, revisar cuenta, contraseña y estado activo. Si da 502, comprobar `API_PROXY_TARGET` y el estado de la API; un servicio gratuito inactivo puede estar despertando.

El correo SMTP es opcional para el login y el tablero. Render bloquea los puertos SMTP habituales en servicios gratuitos; los flujos de correo requieren otra solución o un plan compatible. Los PDFs también deben verificarse en Linux con las fuentes utilizadas por el código existente.

## Verificación local del empaquetado

Con Docker instalado:

```powershell
docker build -t skyport-api .
docker run --rm --env-file RUTA_DE_UN_ARCHIVO_PRIVADO.env -p 10000:10000 skyport-api
```

El archivo privado debe contener `Jwt__Key` y `ConnectionStrings__SkyportDb` o los campos `PG*`. No incluirlo en Git ni en la imagen. Los archivos `.env` de Docker no utilizan el formato `export NOMBRE=...`; usan `NOMBRE=valor`.

También se puede verificar la publicación .NET con `dotnet publish Sky.API/Sky.API.csproj -c Release -o artifacts/render-publish /p:UseAppHost=false`; esto comprueba el proyecto, pero no sustituye la construcción y ejecución de la imagen Docker.

Referencias: [Docker en Render](https://render.com/docs/docker), [Blueprints](https://render.com/docs/blueprint-spec), [proxies en ASP.NET Core](https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-10.0).
