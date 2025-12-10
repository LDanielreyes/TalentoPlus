FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["TalentoPlusWeb/TalentoPlusWeb.csproj", "TalentoPlusWeb/"]
COPY ["TalentoPlus.Application/TalentoPlus.Application.csproj", "TalentoPlus.Application/"]
COPY ["TalentoPlus.Domain/TalentoPlus.Domain.csproj", "TalentoPlus.Domain/"]
COPY ["TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "TalentoPlus.Infrastructure/"]

RUN dotnet restore "TalentoPlusWeb/TalentoPlusWeb.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TalentoPlusWeb"
RUN dotnet build "TalentoPlusWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TalentoPlusWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlusWeb.dll"]
