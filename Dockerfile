FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app
COPY /app/publish .
ENTRYPOINT ["dotnet", "nlp.core.dll"]
EXPOSE 80

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#WORKDIR /src
#COPY ["nlp.core/nlp.core.csproj", "nlp.core/"]
#COPY ["nlp.data/nlp.data.csproj", "nlp.data/"]
#COPY ["nlp.data.text/nlp.data.text.csproj", "nlp.data.text/"]
#COPY ["nlp.services/nlp.services.csproj", "nlp.services/"]
#COPY ["nlp.services.text/nlp.services.text.csproj", "nlp.services.text/"]
#RUN dotnet restore "nlp.core/nlp.core.csproj"
#COPY . .
#WORKDIR "/src/nlp.core"
#RUN dotnet build "nlp.core.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "nlp.core.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "nlp.core.dll"]