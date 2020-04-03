FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app
COPY /app/publish/nlp.core /app/
ENTRYPOINT ["dotnet", "nlp.core.dll"]
EXPOSE 8080