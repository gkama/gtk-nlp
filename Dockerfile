FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["nlp.core/nlp.core.csproj", "nlp.core/"]
COPY ["nlp.data/nlp.data.csproj", "nlp.data/"]
COPY ["nlp.services/nlp.services.csproj", "nlp.services/"]
RUN dotnet restore "nlp.core/nlp.core.csproj"
COPY . .
WORKDIR "/src/nlp.core"
RUN dotnet build "nlp.core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nlp.core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nlp.core.dll"]