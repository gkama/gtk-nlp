FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY /app/publish /app/
ENTRYPOINT ["dotnet", "nlp.core.dll"]
EXPOSE 80