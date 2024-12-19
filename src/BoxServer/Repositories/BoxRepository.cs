using System.Collections.Concurrent;
using CommunityToolkit.Diagnostics;
using BoxServer.Interfaces;
using BoxServer.Models;
using Microsoft.Extensions.Logging;
using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;

namespace BoxServer.Repositories;

public class BoxRepository : IBoxRepository
{
    // Simulate a database, only one instance of the server works for this example
    private static readonly ConcurrentDictionary<int, Box> _boxes = new();
    private readonly HybridCache _cache;
    private readonly ILogger<BoxRepository> _logger;

    public BoxRepository(HybridCache cache, ILogger<BoxRepository> logger)
    {
        _cache = cache;
        _logger = logger;
        for (var i = 0; i < 10; i++)
        {
            Box box = new()
            {
                BoxId = _boxes.Count,
                Name = $"Box {i}",
                Description = $"Description {i}",
                CreatedOn = DateTime.UtcNow - TimeSpan.FromDays(i),
                Active = true
            };
            _boxes.TryAdd(box.BoxId.Value, box);
        }
    }

    public async Task<bool> DeleteBox(int id)
    {
        _boxes.TryRemove(id, out _);
        await _cache.RemoveAsync($"{nameof(Box)}-{id}");
        await _cache.RemoveAsync($"{nameof(Box)}es");
        return true;
    }

    public async Task<Box?> GetBox(int id)
    {
        return await _cache.GetOrCreateAsync(
            $"{nameof(Box)}-{id}", // Unique key to the cache entry
            async (cancel) => await GetBoxFromDb(id, cancel)
        );
    }

    private async Task<Box?> GetBoxFromDb(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CACHE: miss for Box with id {id}", id);
        await Task.CompletedTask;
        Box? ret = _boxes.GetValueOrDefault(id);
        if (ret is null)
            _logger.LogWarning("Box with id {id} not found", id);

        return ret;
    }

    public async Task<IEnumerable<Box>?> GetBoxes()
    {
        return await _cache.GetOrCreateAsync(
            $"{nameof(Box)}es", // Unique key to the cache entry
            async (cancel) => await GetBoxesFromDb(cancel)
        );
    }

    private async Task<IEnumerable<Box>?> GetBoxesFromDb(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CACHE: miss for Boxes");
        await Task.CompletedTask;
        return _boxes.Values;
    }

    public async Task<Box?> AddBox(Box box)
    {
        await Task.CompletedTask;
        Guard.IsNotNull(box);

        box.BoxId = _boxes.Count;
        box.CreatedOn = DateTime.UtcNow;

        var ret = _boxes.TryAdd(box.BoxId.Value, box) ? box : null;
        await _cache.SetAsync($"{nameof(Box)}-{box.BoxId}", box);
        await _cache.RemoveAsync($"{nameof(Box)}es");
        return ret;
    }

    public async Task<Box?> UpdateBox(Box box)
    {
        await Task.CompletedTask;

        Guard.IsNotNull(box);
        Guard.IsNotNull(box.BoxId);

        var ret = _boxes.TryUpdate(box.BoxId.Value, box, _boxes[box.BoxId.Value]) ? box : null;

        await _cache.SetAsync($"{nameof(Box)}-{box.BoxId}", box);
        await _cache.RemoveAsync($"{nameof(Box)}es");
        return ret;

    }
}