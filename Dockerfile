FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Sky.Domain/Sky.Domain.csproj Sky.Domain/
COPY Sky.Application/Sky.Application.csproj Sky.Application/
COPY Sky.Infrastructure/Sky.Infrastructure.csproj Sky.Infrastructure/
COPY Sky.API/Sky.API.csproj Sky.API/
RUN dotnet restore Sky.API/Sky.API.csproj
COPY Sky.Domain/ Sky.Domain/
COPY Sky.Application/ Sky.Application/
COPY Sky.Infrastructure/ Sky.Infrastructure/
COPY Sky.API/ Sky.API/
RUN dotnet publish Sky.API/Sky.API.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
COPY deploy/start-api.sh /app/start-api.sh
RUN chmod 755 /app/start-api.sh
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
USER app
EXPOSE 10000
ENTRYPOINT ["/bin/sh", "/app/start-api.sh"]
