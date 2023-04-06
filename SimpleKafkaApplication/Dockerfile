FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimpleKafkaApplication/SimpleKafkaApplication.csproj", "SimpleKafkaApplication/"]
RUN dotnet restore "SimpleKafkaApplication/SimpleKafkaApplication.csproj"
COPY . .
WORKDIR "/src/SimpleKafkaApplication"
RUN dotnet build "SimpleKafkaApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleKafkaApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleKafkaApplication.dll"]
