#!/bin/sh
set -eu

# Render entrega los campos de PostgreSQL por separado. Npgsql necesita una
# cadena ADO.NET, no la URL postgresql:// que muestra el panel de Render.
quote_npgsql() {
    printf '%s' "$1" | sed 's/"/""/g'
}

if [ -z "${ConnectionStrings__SkyportDb:-}" ]; then
    : "${PGHOST:?Falta PGHOST o ConnectionStrings__SkyportDb}"
    : "${PGDATABASE:?Falta PGDATABASE}"
    : "${PGUSER:?Falta PGUSER}"
    : "${PGPASSWORD:?Falta PGPASSWORD}"
    ConnectionStrings__SkyportDb="Host=\"$(quote_npgsql "$PGHOST")\";Port=${PGPORT:-5432};Database=\"$(quote_npgsql "$PGDATABASE")\";Username=\"$(quote_npgsql "$PGUSER")\";Password=\"$(quote_npgsql "$PGPASSWORD")\";SSL Mode=Require"
    export ConnectionStrings__SkyportDb
fi

: "${Jwt__Key:?Falta Jwt__Key}"
if [ "${#Jwt__Key}" -lt 32 ]; then
    printf '%s\n' 'Jwt__Key debe tener al menos 32 caracteres.' >&2
    exit 1
fi

exec dotnet Sky.API.dll --urls "http://0.0.0.0:${PORT:-10000}"
