FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./src/Scheduling.Reservation ./
RUN dotnet restore
RUN dotnet publish -c Release -o out --runtime linux-x64 --self-contained false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_ENVIRONMENT=Develoment
ENV ReservationDatabase__Username=scheduling_reservation
ENV ReservationDatabase__ConnectionString=mongodb://root:password@mongo-instance:27017
ENV ReservationDatabase__DatabaseName=scheduling
ENV ReservationDatabase__CollectionName=reservations
ENV Logging__Console__FormatterName=simple
ENV TZ=UTC

ENTRYPOINT ["dotnet", "Scheduling.Reservation.dll"]
