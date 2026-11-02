$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repositoryRoot

try {
    $requiredFiles = @(
        'database/01_schema.sql',
        'database/02_identity.sql',
        'database/03_seed.sql',
        'src/SkyPort.Persistence/SkyPort.Persistence.csproj',
        'src/SkyPort.Web/SkyPort.Web.csproj'
    )

    foreach ($file in $requiredFiles) {
        if (-not (Test-Path -LiteralPath $file -PathType Leaf)) {
            throw "Falta el archivo requerido: $file"
        }
    }

    Write-Host 'Restaurando las herramientas .NET locales...'
    dotnet tool restore
    if ($LASTEXITCODE -ne 0) {
        throw 'No se pudieron restaurar las herramientas .NET.'
    }

    $secretList = @(& dotnet user-secrets list --project 'src/SkyPort.Web' 2>$null)
    $hasConfiguredSecret = $secretList | Where-Object { $_ -match '^ConnectionStrings:DefaultConnection\s*=' }
    $hasConfiguredEnvironmentVariable = -not [string]::IsNullOrWhiteSpace($env:ConnectionStrings__DefaultConnection)
    if (-not $hasConfiguredSecret -and -not $hasConfiguredEnvironmentVariable) {
        throw 'Falta ConnectionStrings:DefaultConnection. Configúrala con user-secrets según docs/CONFIGURACION.md; no compartas la clave.'
    }

    $tables = @(
        'aerolinea',
        'aeronave',
        'terminal',
        'puerta',
        'ruta',
        'vuelo',
        'pasajero',
        'reserva',
        'boleto',
        'checkin',
        'embarque',
        'equipaje',
        'empleado',
        'tripulacionvuelo'
    )

    $scaffoldArguments = @(
        'ef', 'dbcontext', 'scaffold',
        'Name=ConnectionStrings:DefaultConnection',
        'Pomelo.EntityFrameworkCore.MySql',
        '--project', 'src/SkyPort.Persistence',
        '--startup-project', 'src/SkyPort.Web',
        '--context', 'SkyPortContext',
        '--context-dir', 'Context',
        '--context-namespace', 'SkyPort.Persistence.Context',
        '--output-dir', '../SkyPort.Domain/Entities',
        '--namespace', 'SkyPort.Domain.Entities',
        '--no-onconfiguring',
        '--no-pluralize',
        '--force'
    )

    foreach ($table in $tables) {
        $scaffoldArguments += @('--table', $table)
    }

    Write-Host 'Generando las entidades y SkyPortContext desde skyport_db...'
    & dotnet @scaffoldArguments
    if ($LASTEXITCODE -ne 0) {
        throw 'Falló el scaffolding. Revisa la conexión local y que skyport_db tenga el esquema de los tres scripts.'
    }
}
finally {
    Pop-Location
}
