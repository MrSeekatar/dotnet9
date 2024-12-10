using System.Collections.Concurrent;
using CommunityToolkit.Diagnostics;
using BoxServer.Interfaces;
using BoxServer.Models;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace BoxServer.Repositories;

public class BoxRepository : IBoxRepository
{
    private readonly ConcurrentDictionary<Guid, Box> _boxes = new();
    private ILogger<BoxRepository> logger;

    public BoxRepository(ILogger<BoxRepository> _logger)
    {
        logger = _logger;
        for (int i = 0; i < 10; i++)
        {
            Box box = new()
            {
                BoxId = NewId.NextGuid(),
                Name = $"Box {i}",
                Description = $"Description {i}",
                CreatedOn = DateTime.UtcNow - TimeSpan.FromDays(i)
            };
            _boxes.TryAdd(box.BoxId.Value, box);
        }
    }

    public async Task<bool> DeleteBox(Guid id)
    {
        await Task.CompletedTask;
        _boxes.TryRemove(id, out _);
        return true;
    }

    public async Task<Box?> GetBox(Guid id)
    {
        await Task.CompletedTask;
        Box? ret = _boxes.GetValueOrDefault(id);
        if ( ret is null)
            logger.LogWarning("Box with id {id} not found", id); // test for logging
        return ret;
    }

    public async Task<IEnumerable<Box>?> GetBoxes()
    {
        await Task.CompletedTask;
        return _boxes.Values;
    }

    public async Task<Box?> AddBox(Box box)
    {
        await Task.CompletedTask;
        Guard.IsNotNull(box);

        box.BoxId = NewId.NextGuid();
        box.CreatedOn = DateTime.UtcNow;

        return _boxes.TryAdd(box.BoxId.Value, box) ? box : null;
    }

    public async Task<Box?> UpdateBox(Box box)
    {
        await Task.CompletedTask;

        Guard.IsNotNull(box);
        Guard.IsNotNull(box.BoxId);

        return _boxes.TryUpdate(box.BoxId.Value, box, _boxes[box.BoxId.Value]) ? box : null;
    }
}