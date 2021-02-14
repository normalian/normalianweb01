#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["normalianweb01/normalianweb01.csproj", "normalianweb01/"]
RUN dotnet restore "normalianweb01/normalianweb01.csproj"
COPY . .
WORKDIR "/src/normalianweb01"
RUN dotnet build "normalianweb01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "normalianweb01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "normalianweb01.dll"]