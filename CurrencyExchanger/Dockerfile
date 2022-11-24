FROM mcr.microsoft.com/mssql/server:2019-latest
COPY ./SetUpDB.sql .

ENV SA_PASSWORD A-a_7276P@a$$wRD!
ENV ACCEPT_EULA Y
ENV MSSQL_TCP_PORT=14330

CMD /bin/bash ./entrypoint.sh
RUN chmod +x /usr/src/app/run-initialization.sh

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CurrencyExchanger.csproj", "./"]
RUN dotnet restore "./CurrencyExchanger.csproj"
COPY . .
RUN dotnet build "CurrencyExchanger.csproj" -c release -o /app

FROM build AS publish
RUN dotnet publish "CurrencyExchanger.csproj" -c release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CurrencyExchanger.dll"]

