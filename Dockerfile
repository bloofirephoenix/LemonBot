FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY . ./

# Restore
RUN dotnet restore

# Build
RUN dotnet publish -c Release -o /app

# Publish
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS publish
WORKDIR /app/data
COPY --from=build /app /app

# Run it
RUN chmod +x ../LemonBot
CMD [ "../LemonBot" ]
