// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;
using Scheduling.Domain.Models;
using Scheduling.Infrastructure.Utils;
using ReservationStatus = Scheduling.Domain.Enums.ReservationStatus;
using ReservationType = Scheduling.Domain.Enums.ReservationType;

string COLLECTION_NAME = "demo-02";
const string accountName = "DunderMifflin";
const string productGroup = "AP";

const int EMPLOYEES_PER_LOCATION = 10;
const int SERVICES_PER_LOCATION = 10;
const int CUSTOMERS_PER_LOCATION = 10;

// TODO: Change this to how many days into the future you want to seed
int DAYS_IN_FUTURE = 365;

// TODO: Change this to add a dataset for a different location
var locationId = "1";

// TODO: Change this to edit the duration of all appointments
var DURATION = 15;


BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.Document));
var watch = new System.Diagnostics.Stopwatch();
var dbClient = new MongoClient("mongodb://localhost:27017/");
var reservations = new ConcurrentQueue<WriteModel<ReservationEntity>>();

long current = 0;
var today = DateTime.Now;

watch.Start();
Parallel.For(0, DAYS_IN_FUTURE, days =>
{
    Parallel.For(0, EMPLOYEES_PER_LOCATION, employeeIdx =>
    {
        
        DateTime startDate = new DateTime(today.Year, today.Month, 1, 9, 0, 0).AddDays(days);
        var random = new Random();
        
        
        var serviceIdentifier = random.Next(1, SERVICES_PER_LOCATION + 1);
        var customerIdentifier = random.Next(1, CUSTOMERS_PER_LOCATION + 1);
        
        do
        {
            DateTime endDate = startDate.AddMinutes(DURATION);

            var employeeId = $"{locationId}{employeeIdx}";
            var serviceId = $"{locationId}{serviceIdentifier}";
            var customerId = $"{locationId}{customerIdentifier}";
            
            try
            {
                var newReservation = new ReservationEntity
                {
                    Account = accountName,
                    ProductGroup = productGroup,
                    StartTime = startDate,
                    EndTime = endDate,
                    Duration = DURATION,
                    Spots = 1,
                    Status = ReservationStatus.Open,
                    Type = ReservationType.Appointment,
                    AssignmentDetails = new List<AssignmentSummary>(),
                    Assignments = new string[] { $"{(int)AssignmentType.Location}-{locationId}", $"{(int)AssignmentType.Employee}-{employeeId}", $"{(int)AssignmentType.Service}-{serviceId}", $"{(int)AssignmentType.Customer}-{customerId}" }
                };
                
                newReservation.AssignmentDetails.Add(new AssignmentSummary
                {
                    Id = new ObjectId().ToString(),
                    Key = locationId,
                    Name = $"Location_{locationId}",
                    Type = (int)AssignmentType.Location
                    
                });

                newReservation.AssignmentDetails.Add(new AssignmentSummary
                {
                    Id = new ObjectId().ToString(),
                    Key = $"{serviceId}",
                    Name = $"Service_{serviceId}",
                    Type = (int)AssignmentType.Service
                });

                newReservation.AssignmentDetails.Add(new AssignmentSummary
                {
                    Id = new ObjectId().ToString(),
                    Key = $"{employeeIdx}",
                    Name = $"Employee_{employeeIdx}",
                    Type = (int)AssignmentType.Employee
                });

                newReservation.AssignmentDetails.Add(new AssignmentSummary
                {
                    Id = new ObjectId().ToString(),
                    Key = $"{customerId}",
                    Name = $"Customer_{customerId}",
                    Type = (int)AssignmentType.Customer
                });

                reservations.Enqueue(new InsertOneModel<ReservationEntity>(newReservation));
            }
            finally
            {
                Interlocked.Increment(ref current);
            }

            startDate = endDate;

        } while (startDate.Hour < 17);
    });
    
});


var resDb = dbClient.GetDatabase($"scheduling");

var collection = CollectionManager.CreateReservationCollection(resDb, COLLECTION_NAME);
    
//var resCollection = resDb.GetCollection<Reservation>($"iteration_1");
var resultWrites = await collection.BulkWriteAsync(reservations);
Console.WriteLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}");
watch.Stop();

TimeSpan timeSpan = watch.Elapsed;
Console.WriteLine("Time: {0}h {1}m {2}s {3}ms", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);