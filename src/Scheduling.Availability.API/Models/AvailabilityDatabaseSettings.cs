namespace Scheduling.Availability.API.Models;

public class AvailabilityDatabaseSettings
{
    public const string SettingsKey = @"AvailabilityDatabase";
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string CollectionName { get; set; }
}
