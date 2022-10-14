using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Scheduling.Application.Interfaces;
using Scheduling.Availability.API.Extensions;
using Scheduling.Availability.API.Models;
using Scheduling.Infrastructure;
using Scheduling.Infrastructure.Interfaces;
using Scheduling.Infrastructure.Repositories;
using Scheduling.Reservation.Models;


var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true)
    .AddEnvironmentVariables();

// Grab reference to container
var services = builder.Services;

// Add IHttpContextAccessor resolver
services.AddHttpContextAccessor();

// Add Controllers to the container.
services.AddControllers();

services.Configure<AwsConfigurationSettings>(
    builder.Configuration.GetSection(AwsConfigurationSettings.SettingsKey));

// services.AddAwsSpecificDependencies();
// services.AddSingleton<ICloudConfiguration, AwsCloudConfiguration>();

// Setup container with app services
services.AddScoped<IAssignmentService, AssignmentService>();
services.AddScoped<IAssignmentRepository, AssignmentRepository>();


// Setup configuration sections for use with IOptions
services.Configure<AvailabilityDatabaseSettings>(
    builder.Configuration.GetSection(AvailabilityDatabaseSettings.SettingsKey));

services.AddRepositoryServices();

BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.Document));

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Scheduling Availability Service");


app.Run();
