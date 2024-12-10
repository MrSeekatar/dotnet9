using BoxServer.Interfaces;
using BoxServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BoxServer.Services;

public partial class BoxProcessor : IBoxProcessor
{
    public partial int BoxesProcessed { get; set; } // C# 13 partial on property
}