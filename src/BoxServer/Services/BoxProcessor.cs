using BoxServer.Interfaces;
using BoxServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BoxServer.Services;

public partial class BoxProcessor : IBoxProcessor
{
    private readonly Lock _lock = new();
    public partial int BoxesProcessed // C# 13 partial on property
    {
        get;
        set
        {
            // contrived validation example
            if (value > 0)
            {
                field += value; // C# 13 use the backing field created by get
            }
        }
    }

    public int BoxesNotProcessed
    {
        get => field < 0 ? 0 : field; // C# 13 use the backing field created by set
        set;
    }

    public void ProcessBoxes(params IList<Box> boxes) // C# 13 enumerable params
    {
        lock (_lock)
        {
            BoxesProcessed += boxes.Count(o => o.Active);
            BoxesNotProcessed += boxes.Count(o => !o.Active);
        }
    }
}