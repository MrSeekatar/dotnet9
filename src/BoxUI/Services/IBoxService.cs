using BoxServer.Models;
using Refit;

namespace BoxUI.Services;

// service interface for the Box service. Refit will generate the implementation
internal interface IBoxService
{
    [Get("/box")]
    Task<List<Box>> GetBoxes();

    [Get("/box/{boxId}")]
    Task<Box> GetBox(int boxId);

    [Delete("/box/{boxId}")]
    Task DeleteBox(int boxId);

    [Post("/box")]
    Task AddBox([Body]Box newBox);
}