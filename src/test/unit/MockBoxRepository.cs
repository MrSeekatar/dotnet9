using BoxServer.Interfaces;
using BoxServer.Models;
using MassTransit;

namespace unit;

public partial class MockBoxRepository : IBoxRepository
{

    public async Task<bool> DeleteBox(string clientId, Guid Id)
    {
        var box = await GetBox(clientId, Id).ConfigureAwait(false);
        if (box == null || box.BoxId == null)
        {
            return false;
        }
        _map.Remove(box.BoxId!.ToString()!.ToUpperInvariant());
        return true;
    }

    public async Task<Box?> GetBox(string clientId, Guid id)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        return _map.TryGetValue(id.ToString().ToUpperInvariant(), out var box) ? box : null;
    }

    public async Task<IEnumerable<Box>?> GetBoxs(string clientId)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        return _map.Values;
    }

    public async Task<Box> AddBox(string clientId, Box box)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        box.BoxId = NewId.NextGuid();
        box.CreatedOn = DateTime.UtcNow;
        _map[box.BoxId!.ToString()!.ToUpperInvariant()] = box;

        return box;
    }

    public async Task<Box?> UpdateBox(string clientId, Box box)
    {
        if (box.BoxId is null || await GetBox(clientId, box.BoxId.Value).ConfigureAwait(false) is null) return null;
        _map[box.BoxId!.ToString()!.ToUpperInvariant()] = box;
        return box;
    }
}