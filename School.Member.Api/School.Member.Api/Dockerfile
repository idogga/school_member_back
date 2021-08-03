#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["School.Member.Api/School.Member.Api.csproj", "School.Member.Api/"]
RUN dotnet restore "School.Member.Api/School.Member.Api.csproj"
COPY . .
WORKDIR "/src/School.Member.Api"
RUN dotnet build "School.Member.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "School.Member.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "School.Member.Api.dll"]