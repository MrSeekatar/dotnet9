using BoxServer.Models;
using Refit;

namespace BoxUI.Services;

internal interface IBoxService
{
    [Get("/box")]
    Task<List<Box>> GetBoxes();

    [Get("/box/{boxId}")]
    Task<Box> GetBox(int boxId);

    [Post("/box")]
    Task AddBox([Body]Box newBox);
}