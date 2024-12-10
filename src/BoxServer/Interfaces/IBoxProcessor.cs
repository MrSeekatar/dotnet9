using BoxServer.Models;

namespace BoxServer.Interfaces;

interface IBoxProcessor
{
    void ProcessBoxes(params IList<Box> boxes);

    int BoxesProcessed { get; set;}

    int BoxesNotProcessed { get; set;}
}