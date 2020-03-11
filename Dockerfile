FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
COPY /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "nlp.core.dll"]