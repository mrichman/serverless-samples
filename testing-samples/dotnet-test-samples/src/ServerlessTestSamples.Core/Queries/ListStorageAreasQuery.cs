using ServerlessTestSamples.Core.Services;
using Microsoft.Extensions.Logging;

namespace ServerlessTestSamples.Core.Queries;

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
    private readonly ILogger<ListStorageAreasQueryHandler> _logger;
    

    public ListStorageAreasQueryHandler(IStorageService storageService, ILogger<ListStorageAreasQueryHandler> logger)
    {
        _storageService = storageService;
        _logger = logger;
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