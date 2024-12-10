using System.Diagnostics.CodeAnalysis;
using BoxServer.Models;

namespace unit;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
public partial class MockBoxRepository
{
    private Dictionary<string, Box> _map = new()
    {
        {"7D259C54-3272-4147-B292-F77046211AED", new Box() {BoxId = Guid.Parse("7D259C54-3272-4147-B292-F77046211AED"), Name = "BoxA", Active = true}},
        {"2194D6D4-0398-41D9-946E-038B88D30152", new Box() {BoxId = Guid.Parse("2194D6D4-0398-41D9-946E-038B88D30152"), Name = "BoxB", Active = true}},
        {"622F4226-F2B0-4843-83B4-A536D1141597", new Box() {BoxId = Guid.Parse("622F4226-F2B0-4843-83B4-A536D1141597"), Name = "BoxC", Active = true}}
    };
}