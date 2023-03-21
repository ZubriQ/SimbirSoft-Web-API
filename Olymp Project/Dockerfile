#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Olymp Project/Olymp Project.csproj", "Olymp Project/"]
RUN dotnet restore "Olymp Project/Olymp Project.csproj"
COPY . .
WORKDIR "/src/Olymp Project"
RUN dotnet build "Olymp Project.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Olymp Project.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Olymp Project.dll"]
