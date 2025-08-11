# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# copy csproj files
COPY Bank.API/Bank.API.csproj Bank.API/
COPY Bank.BLL/Bank.BLL.csproj Bank.BLL/
COPY Bank.DAL/Bank.DAL.csproj Bank.DAL/
COPY Bank.Core/Bank.Core.csproj Bank.Core/

# restore
RUN dotnet restore Bank.API/Bank.API.csproj

# copy source code
COPY Bank.API/ Bank.API/
COPY Bank.BLL/ Bank.BLL/
COPY Bank.DAL/ Bank.DAL/
COPY Bank.Core/ Bank.Core/

# build
WORKDIR /src/Bank.API
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

ENV ASPNETCORE_URLS=http://+:5001
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5001

ENTRYPOINT ["dotnet", "Bank.API.dll"]