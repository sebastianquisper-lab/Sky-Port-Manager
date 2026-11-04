<#
.SYNOPSIS
    Genera entidades operativas y SkyPortContext en un directorio de revision,
    muestra diferencias frente a los archivos versionados y solo los reemplaza
    con -Apply tras la revision.

.PARAMETER Apply
    Copia el resultado revisado sobre Domain/Entities y Persistence/Context.
    Sin este interruptor no se sobrescriben archivos versionados.
#>
[CmdletBinding()]
param(
    [switch]$Apply
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repositoryRoot

try {
    $requiredFiles = @(
        'database/01_schema.sql',
        'database/02_identity.sql',
        'database/03_seed.sql',
        'src/SkyPort.Persistence/SkyPort.Persistence.csproj',
        'src/SkyPort.Domain/SkyPort.Domain.csproj',
        'src/SkyPort.Web/SkyPort.Web.csproj'
    )

    foreach ($file in $requiredFiles) {
        if (-not (Test-Path -LiteralPath $file -PathType Leaf)) {
            throw "Falta el archivo requerido: $file"
        }
    }

    $reviewRoot = Join-Path $repositoryRoot 'artifacts/scaffold-review'
    $reviewEntities = Join-Path $reviewRoot 'Entities'
    $reviewContext = Join-Path $reviewRoot 'Context'
    $versionedEntities = Join-Path $repositoryRoot 'src/SkyPort.Domain/Entities'
    $versionedContext = Join-Path $repositoryRoot 'src/SkyPort.Persistence/Context'

    Write-Host 'Restaurando las herramientas .NET locales...'
    dotnet tool restore
    if ($LASTEXITCODE -ne 0) {
        throw 'No se pudieron restaurar las herramientas .NET.'
    }

    $secretList = @(& dotnet user-secrets list --project 'src/SkyPort.Web' 2>$null)
    $hasConfiguredSecret = $secretList | Where-Object { $_ -match '^ConnectionStrings:DefaultConnection\s*=' }
    $hasConfiguredEnvironmentVariable = -not [string]::IsNullOrWhiteSpace($env:ConnectionStrings__DefaultConnection)
    if (-not $hasConfiguredSecret -and -not $hasConfiguredEnvironmentVariable) {
        throw 'Falta ConnectionStrings:DefaultConnection. Configurala con user-secrets segun docs/CONFIGURACION.md; no compartas la clave.'
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

    if (Test-Path -LiteralPath $reviewRoot) {
        Remove-Item -LiteralPath $reviewRoot -Recurse -Force
    }
    New-Item -ItemType Directory -Path $reviewEntities -Force | Out-Null
    New-Item -ItemType Directory -Path $reviewContext -Force | Out-Null

    # Rutas relativas al proyecto Persistence; artifacts/ esta fuera de src/ y no se compila.
    $scaffoldArguments = @(
        'ef', 'dbcontext', 'scaffold',
        'Name=ConnectionStrings:DefaultConnection',
        'Pomelo.EntityFrameworkCore.MySql',
        '--project', 'src/SkyPort.Persistence',
        '--startup-project', 'src/SkyPort.Web',
        '--context', 'SkyPortContext',
        '--context-dir', '../../artifacts/scaffold-review/Context',
        '--context-namespace', 'SkyPort.Persistence.Context',
        '--output-dir', '../../artifacts/scaffold-review/Entities',
        '--namespace', 'SkyPort.Domain.Entities',
        '--no-onconfiguring',
        '--no-pluralize',
        '--force'
    )

    foreach ($table in $tables) {
        $scaffoldArguments += @('--table', $table)
    }

    Write-Host 'Generando revision en artifacts/scaffold-review (sin tocar archivos versionados)...'
    & dotnet @scaffoldArguments
    if ($LASTEXITCODE -ne 0) {
        throw 'Fallo el scaffolding de revision. Revisa la conexion local y que skyport_db tenga el esquema operativo.'
    }

    function Get-RelativeRepoPath {
        param([string]$FullPath)
        return $FullPath.Substring($repositoryRoot.Length).TrimStart('\', '/')
    }

    function Show-FileDiffs {
        param(
            [string]$ReviewDirectory,
            [string]$VersionedDirectory,
            [string]$Label
        )

        Write-Host ''
        Write-Host ('=== Diferencias: ' + $Label + ' ===')

        if (-not (Test-Path -LiteralPath $VersionedDirectory)) {
            Write-Host ('No existe el directorio versionado: ' + $VersionedDirectory)
            return
        }

        $reviewFiles = Get-ChildItem -LiteralPath $ReviewDirectory -File -Filter '*.cs' | Sort-Object Name
        $versionedFiles = Get-ChildItem -LiteralPath $VersionedDirectory -File -Filter '*.cs' | Sort-Object Name
        $reviewNames = @($reviewFiles | ForEach-Object { $_.Name })
        $versionedNames = @($versionedFiles | ForEach-Object { $_.Name })

        $onlyInReview = @(Compare-Object $versionedNames $reviewNames |
            Where-Object { $_.SideIndicator -eq '=>' } |
            ForEach-Object { $_.InputObject })
        $onlyInVersioned = @(Compare-Object $versionedNames $reviewNames |
            Where-Object { $_.SideIndicator -eq '<=' } |
            ForEach-Object { $_.InputObject })

        foreach ($name in $onlyInReview) {
            Write-Host ('  [NUEVO] ' + $name)
        }
        foreach ($name in $onlyInVersioned) {
            Write-Host ('  [AUSENTE EN REVISION] ' + $name)
        }

        $common = @($reviewNames | Where-Object { $versionedNames -contains $_ })
        $changed = 0
        foreach ($name in $common) {
            $left = Join-Path $VersionedDirectory $name
            $right = Join-Path $ReviewDirectory $name
            $diff = & git --no-pager diff --no-index --unified=3 -- $left $right 2>$null
            if ($LASTEXITCODE -ne 0 -and $diff) {
                $changed++
                Write-Host ''
                Write-Host ('  --- ' + (Get-RelativeRepoPath $left))
                Write-Host ('  +++ artifacts/scaffold-review/' + $name)
                $diff | Select-Object -Skip 2 | ForEach-Object { Write-Host ('  ' + $_) }
            }
        }

        if ($changed -eq 0 -and $onlyInReview.Count -eq 0 -and $onlyInVersioned.Count -eq 0) {
            Write-Host '  Sin diferencias.'
        }
    }

    Show-FileDiffs -ReviewDirectory $reviewEntities -VersionedDirectory $versionedEntities -Label 'Entidades (Domain/Entities)'
    Show-FileDiffs -ReviewDirectory $reviewContext -VersionedDirectory $versionedContext -Label 'Contexto (Persistence/Context)'

    if (-not $Apply) {
        Write-Host ''
        Write-Host 'Revision generada. Los archivos versionados no se modificaron.'
        Write-Host 'Tras revisar las diferencias, vuelve a ejecutar con -Apply para reemplazarlos:'
        Write-Host '  .\tools\scaffold.ps1 -Apply'
        return
    }

    Write-Host ''
    Write-Host 'Aplicando la revision sobre los archivos versionados...'

    if (-not (Test-Path -LiteralPath $versionedEntities)) {
        New-Item -ItemType Directory -Path $versionedEntities -Force | Out-Null
    }
    if (-not (Test-Path -LiteralPath $versionedContext)) {
        New-Item -ItemType Directory -Path $versionedContext -Force | Out-Null
    }

    Get-ChildItem -LiteralPath $reviewEntities -File -Filter '*.cs' | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $versionedEntities $_.Name) -Force
    }
    Get-ChildItem -LiteralPath $reviewContext -File -Filter '*.cs' | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $versionedContext $_.Name) -Force
    }

    Write-Host 'Archivos versionados actualizados desde artifacts/scaffold-review.'
    Write-Host 'Identity no se regenera aqui; conserva SkyPortIdentityDbContext y database/02_identity.sql.'
}
finally {
    Pop-Location
}
