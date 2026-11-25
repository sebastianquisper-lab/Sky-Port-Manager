# SkyPort Manager
Sistema de gestión aeroportuaria desarrollado con .NET 10, Entity Framework Core y PostgreSQL.

## Despliegue en Render

La API y el frontend se publican como servicios separados. Este repositorio incluye `Dockerfile` y `render.yaml` para desplegar la API y crear una base PostgreSQL, sin modificar `Program.cs` ni reorganizar los proyectos.

**[Pasos para desplegar, importar la base y conectar el frontend](RENDER.md)**

El Blueprint crea una base vacía: hay que importar el esquema y los datos originales del equipo. El README no contiene la base de datos y este repositorio no incluye scripts SQL ni migraciones para reconstruirla.

## Interfaz web (frontend)

El frontend se desarrolla en un repositorio independiente para mantener este proyecto dedicado a la API.

**[Ver el código y las instrucciones del frontend: Sky-Port-Frontend](https://github.com/johanvallejos-cyber/Sky-Port-Frontend)**

Incluye inicio de sesión, panel de operaciones y consulta de vuelos. Su README explica cómo ejecutarlo y conectarlo a esta API. El enlace lleva al código fuente; la interfaz todavía no está publicada como sitio web.

## Integrantes
- Quispe Riquelme Sebastian (líder y backend)
- Johan David Vallejos Vilcape (base de datos y Entity Framework)
- Kalhel Eithan Gómez Avalos (interfaz de usuario)
- Victor Manuel Huacacha Ccahua (pruebas y documentación)
