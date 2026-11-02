# Permisos de SkyPort Manager

La aplicación exige un usuario autenticado como política de respaldo. Las políticas de rol definen qué operaciones puede realizar ese usuario. `RequireRole` permite el acceso si el usuario pertenece a cualquiera de los roles indicados en la política.

## Roles y políticas

| Política | Roles permitidos |
| --- | --- |
| `Policies.SoloAdmin` | `Roles.Administrador` |
| `Policies.GestionOperaciones` | `Roles.Administrador`, `Roles.OperadorVuelos` |
| `Policies.GestionPasajeros` | `Roles.Administrador`, `Roles.AgenteCheckIn` |
| `Policies.TodosLosRoles` | `Roles.Administrador`, `Roles.OperadorVuelos`, `Roles.AgenteCheckIn` |

Usa las constantes de `SkyPort.Application.Common.Constants.Roles` y `Policies`. No escribas los nombres de roles ni políticas como cadenas literales en los controladores.

## Matriz por módulo

La siguiente matriz reproduce los permisos acordados en la sección 4 del plan. Las columnas de escritura y lectura nombran la política que debe aplicarse.

| Módulo | Escritura | Lectura |
| --- | --- | --- |
| Usuarios | `SoloAdmin` | `SoloAdmin` |
| Aerolíneas y aeronaves | `SoloAdmin` | `GestionOperaciones` |
| Rutas y vuelos | `GestionOperaciones` | `TodosLosRoles` |
| Personal | `SoloAdmin` | `GestionOperaciones` |
| Tripulación | `GestionOperaciones` | `GestionOperaciones` |
| Pasajeros, reservas y boletos | `GestionPasajeros` | `TodosLosRoles` |
| Check-in y embarque | `GestionPasajeros` | `TodosLosRoles` |
| Equipaje | `GestionPasajeros` | `TodosLosRoles` |
| Reportes | No aplica | `TodosLosRoles` |

## Atributos por controlador

Aplica el atributo de lectura a las acciones de consulta y el de escritura a las acciones que crean, actualizan, eliminan o cambian estado. Cuando todas las acciones de un controlador usan la misma política, puede ponerse en la clase; si cambian por acción, ponlo en cada acción.

| Controlador | Acciones de escritura | Acciones de lectura |
| --- | --- | --- |
| `UsuariosController` | `[Authorize(Policy = Policies.SoloAdmin)]` | `[Authorize(Policy = Policies.SoloAdmin)]` |
| `AerolineasController` | `[Authorize(Policy = Policies.SoloAdmin)]` | `[Authorize(Policy = Policies.GestionOperaciones)]` |
| `AeronavesController` | `[Authorize(Policy = Policies.SoloAdmin)]` | `[Authorize(Policy = Policies.GestionOperaciones)]` |
| `RutasController` | `[Authorize(Policy = Policies.GestionOperaciones)]` | `[Authorize(Policy = Policies.TodosLosRoles)]` |
| `VuelosController` | `[Authorize(Policy = Policies.GestionOperaciones)]` | `[Authorize(Policy = Policies.TodosLosRoles)]` |
| `PersonalController` | `[Authorize(Policy = Policies.SoloAdmin)]` | `[Authorize(Policy = Policies.GestionOperaciones)]` |
| `TripulacionController` | `[Authorize(Policy = Policies.GestionOperaciones)]` | `[Authorize(Policy = Policies.GestionOperaciones)]` |
| `PasajerosController`, `ReservasController`, `BoletosController` | `[Authorize(Policy = Policies.GestionPasajeros)]` | `[Authorize(Policy = Policies.TodosLosRoles)]` |
| `CheckInController`, `EmbarqueController`, `EquipajeController` | `[Authorize(Policy = Policies.GestionPasajeros)]` | `[Authorize(Policy = Policies.TodosLosRoles)]` |
| `ReportesController` | No aplica | `[Authorize(Policy = Policies.TodosLosRoles)]` |

Las acciones POST que cambian datos también deben llevar antiforgery, además de la política correspondiente.

## Política de respaldo y página principal

`AddSkyPortAuthorization` configura una política de respaldo que exige autenticación en todas las rutas que no tengan `[AllowAnonymous]`. `Login` y `AccessDenied` son excepciones públicas del flujo de identidad. `HomeController` requiere uno de los tres roles para mostrar la portada:

```csharp
[Authorize(Policy = Policies.TodosLosRoles)]
public class HomeController : Controller
```

Un usuario autenticado sin ninguno de los tres roles no puede abrir Home. Las rutas de login permanecen públicas para que ese usuario pueda iniciar sesión.
