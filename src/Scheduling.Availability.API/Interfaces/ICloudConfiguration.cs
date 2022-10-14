namespace Scheduling.Availability.API.Interfaces;

public interface ICloudConfiguration
{
    string GetConfigurationValue(string key);
}
