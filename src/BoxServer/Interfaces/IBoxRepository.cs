using BoxServer.Models;

namespace BoxServer.Interfaces;

public interface IBoxRepository
{
    Task<bool> DeleteBox(Guid id);
    Task<Box?> AddBox(Box box);
    Task<Box?> UpdateBox(Box box);
    Task<IEnumerable<Box>?> GetBoxes();
    Task<Box?> GetBox(Guid id);
}