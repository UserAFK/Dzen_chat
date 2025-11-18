# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Dzen_chat.Api/Dzen_chat.Api.csproj Dzen_chat.Api/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

RUN dotnet restore "Dzen_chat.Api/Dzen_chat.Api.csproj"
COPY Dzen_chat.Api/ Dzen_chat.Api/
COPY Application/ Application/
COPY Infrastructure/ Infrastructure/

RUN dotnet publish "Dzen_chat.Api/Dzen_chat.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
EXPOSE 443
ENTRYPOINT ["dotnet", "Dzen_chat.Api.dll"]
