using BoxUI.Models;
using Refit;

namespace BoxUI.Services;

internal interface IBoxService
{
    [Get("/box")]
    Task<List<Box>> GetBoxes();

}