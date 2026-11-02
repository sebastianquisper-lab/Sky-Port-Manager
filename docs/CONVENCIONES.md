# Convenciones de SkyPort Manager

## Stack
ASP.NET Core MVC (Controllers y Views), MediatR, EF Core con SQL Server y ASP.NET Core Identity. Enfoque base de datos primero: la base se crea con los scripts de database/ y las entidades se generan con scaffold. No se usa Code First ni migraciones.

## Estructura
src/SkyPort.Domain, src/SkyPort.Application, src/SkyPort.Persistence, src/SkyPort.Infrastructure, src/SkyPort.Web, tests/SkyPort.Tests, database/, docs/ y tools/.

## Dependencias entre proyectos (arquitectura hexagonal)
- Domain: no referencia a ningún proyecto. Contiene Entities (generadas por scaffold, no se editan a mano), Ports (IGenericRepository, IUnitOfWork) y Exceptions.
- Application: referencia a Domain. Contiene UseCases/<Modulo>/Commands, Queries y Dtos; Common (Result y constantes) y Ports (puertos que devuelven DTOs: I<Modulo>Queries, IReportExporter, IUserAccountService).
- Persistence: referencia a Domain. Contiene SkyPortContext (scaffold, no se edita a mano) y SkyPortIdentityDbContext.
- Infrastructure: referencia a Domain, Application y Persistence. Implementa los puertos: repositorios, UnitOfWork, Queries, exportadores y servicio de usuarios.
- Web: referencia a Application e Infrastructure (Infrastructure solo para registrar servicios en Program.cs). Contiene Controllers, Models (ViewModels) y Views.
- Tests: referencia a todos los proyectos.

## Reglas
1. Los controladores son delgados: reciben el ViewModel, envían un comando o consulta con IMediator y muestran el resultado. No contienen reglas de negocio ni usan DbContext.
2. Los handlers usan IUnitOfWork o puertos de Application/Ports. Nunca usan DbContext directamente.
3. Las reglas de negocio (choques de horario, sobreventa, asiento único, cambios de estado) viven en Application. Las restricciones de la base son la última defensa.
4. Los handlers devuelven Result o Result<T> con un error de tipo NotFound, Validation, Conflict o Forbidden. El controlador lo convierte en vista o mensaje.
5. Consultas con joins, filtros o agregaciones: puerto I<Modulo>Queries en Application/Ports e implementación con LINQ en Infrastructure/Queries. Las clases de Infrastructure/Queries y Infrastructure/Services se registran solas por reflexión; no se edita Program.cs.
6. ViewModels con Data Annotations en Web/Models. DTOs como records en Application/UseCases/<Modulo>/Dtos.
7. Estados, roles y políticas son constantes en Application/Common (EstadoVuelo, EstadoReserva, Roles, Policies, etc.). No se escriben como texto suelto.
8. Entidades y tablas en español singular (Vuelo, Aeronave). Sufijos técnicos en inglés (CreateVueloCommand, VuelosController).
9. Las fechas se guardan en hora local de Lima, sin conversión de zona horaria.
10. Si cambia la base: se edita el script de database/, se recrea la base local y Sebas o Johan corren tools/scaffold.ps1 avisando al grupo. Nadie edita SkyPortContext ni Domain/Entities a mano.

## Seguridad
Ningún secreto en el repositorio. La cadena de conexión y los datos del administrador inicial van en user-secrets (ver docs/CONFIGURACION.md).

## Commits
tipo(módulo): descripción en inglés e imperativo. Un commit por tarea. Ramas feat/<modulo>-<tema>.
