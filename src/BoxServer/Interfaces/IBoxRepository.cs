using BoxServer.Models;

namespace BoxServer.Interfaces;

public interface IBoxRepository
{
    Task<bool> DeleteBox(int id);
    Task<Box?> AddBox(Box box);
    Task<Box?> UpdateBox(Box box);
    Task<IEnumerable<Box>?> GetBoxes();
    Task<Box?> GetBox(int id);
}