FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ContactManagement.sln", "./"]
COPY ["src/ContactManagement.API/ContactManagement.API.csproj", "src/ContactManagement.API/"]
COPY ["src/ContactManagement.Application/ContactManagement.Application.csproj", "src/ContactManagement.Application/"]
COPY ["src/ContactManagement.Domain/ContactManagement.Domain.csproj", "src/ContactManagement.Domain/"]
COPY ["src/ContactManagement.Infrastructure/ContactManagement.Infrastructure.csproj", "src/ContactManagement.Infrastructure/"]
COPY ["src/ContactManagement.Shared/ContactManagement.Shared.csproj", "src/ContactManagement.Shared/"]
COPY ["tests/ContactManagement.UnitTests/ContactManagement.UnitTests.csproj", "tests/ContactManagement.UnitTests/"]
COPY ["tests/ContactManagement.IntegrationTests/ContactManagement.IntegrationTests.csproj", "tests/ContactManagement.IntegrationTests/"]
COPY ["tests/ContactManagement.FunctionalTests/ContactManagement.FunctionalTests.csproj", "tests/ContactManagement.FunctionalTests/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/src/ContactManagement.API"
RUN dotnet build "ContactManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "ContactManagement.API.dll"]
