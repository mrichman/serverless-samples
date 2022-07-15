using ServerlessTestSamples.Core.Services;

namespace ServerlessTestSamples.Core;

public class ListStorageAreasQuery
{
    public string FilterPrefix { get; set; }
}

public class ListStorageAreasQueryResult
{
    public IEnumerable<string> StorageAreas { get; set; }
}

public class ListStorageAreasQueryHandler
{
    private readonly IStorageService _storageService;

    public ListStorageAreasQueryHandler(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<ListStorageAreasQueryResult> Handle(ListStorageAreasQuery query)
    {
        try
        {
            var storageAreas = await _storageService.ListStorageAreas(query.FilterPrefix);

            if (storageAreas == null)
            {
                storageAreas = new List<string>(0);
            }

            return new ListStorageAreasQueryResult
            {
                StorageAreas = storageAreas
            };
        }
        catch (Exception ex)
        {
            // Add logging/metrics here.
            return new ListStorageAreasQueryResult
            {
                StorageAreas = new List<string>(0),
            };
        }
    }
}