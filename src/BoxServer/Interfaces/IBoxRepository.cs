using BoxServer.Models;

namespace BoxServer.Interfaces;

public interface IBoxRepository
{
    Task<bool> DeleteBox(string clientId, Guid id);
    Task<Box> AddBox(string clientId, Box box);
    Task<Box?> UpdateBox(string clientId, Box box);
    Task<IEnumerable<Box>?> GetBoxs(string clientId);
    Task<Box?> GetBox(string clientId, Guid id);
}