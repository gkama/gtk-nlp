FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster
WORKDIR /app
COPY ./app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "nlp.core.dll"]