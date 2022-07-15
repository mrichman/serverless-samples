namespace ServerlessTestSamples.Core.Services;

public interface IStorageService
{
    Task<IEnumerable<string>> ListStorageAreas(string filterPrefix);
}