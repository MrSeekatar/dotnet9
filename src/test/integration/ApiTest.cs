using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using BoxServer;
using BoxServer.Models;
using Loyal.Core.Test;
using Loyal.Core.Util;
using static unit.TestConstants;

namespace ApiTest;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
public partial class ApiTests : XUnitHttpClientTestBase<ApiTests>
{
    [Theory]
    [InlineData(BoxAId)]
    [InlineData(BoxBId)]
    [InlineData(BoxCId)]
    public async Task GetBox(string id)
    {
        var box = await HttpClient!.GetFromJsonAsync<Box>($"{UriPrefix}/api/v1/{ScrantonClientId}/box/{id}",
            CustomSerializerOptions.Options);
        box.ShouldNotBeNull();
        box.BoxId.ShouldBe(Guid.Parse(id));
    }

    [Fact]
    public async Task GetBoxs()
    {
        var boxs = await HttpClient!.GetFromJsonAsync<IEnumerable<Box>>($"{UriPrefix}/api/v1/{ScrantonClientId}/box",
            CustomSerializerOptions.Options);
        boxs.ShouldNotBeNull();
        boxs.Count().ShouldBeGreaterThan(2);
        boxs.ShouldContain(o => o.BoxId == Guid.Parse(BoxAId));
        boxs.ShouldContain(o => o.BoxId == Guid.Parse(BoxBId));
        boxs.ShouldContain(o => o.BoxId == Guid.Parse(BoxCId));
    }

    [Fact]
    public async Task AddUpdateDeleteBox()
    {
        var newBox = new Box()
        {
            Name = $"Test-{Guid.NewGuid().ToString().Replace('-','_')}",
            Description = "~~Added by unit test~~",
            Active = true
        };
        var response = await HttpClient!.PostAsJsonWithReply<Box,Box>($"{UriPrefix}/api/v1/{ScrantonClientId}/box", newBox, DefaultSerializationOptions);
        response.ShouldSatisfyAllConditions(
            () => response.ShouldNotBeNull(),
            () => response.BoxId.ShouldNotBeNull(),
            () => response.BoxId.ShouldNotBe(Guid.Empty)
            );

        var id = response.BoxId!.Value;
        var box = await HttpClient!.GetFromJsonAsync<Box>($"{UriPrefix}/api/v1/{ScrantonClientId}/box/{id}",
            CustomSerializerOptions.Options);
        box.ShouldNotBeNull();

        box.Description = "~~Updated by unit test~~";
        box = await HttpClient!.PutAsJsonWithReply<Box,Box>($"{UriPrefix}/api/v1/{ScrantonClientId}/box", box, DefaultSerializationOptions);
        box.ShouldNotBeNull();

        box = await HttpClient!.GetFromJsonAsync<Box>($"{UriPrefix}/api/v1/{ScrantonClientId}/box/{id}",
            CustomSerializerOptions.Options);
        box.ShouldSatisfyAllConditions(
            () => box.ShouldNotBeNull(),
            () => box!.Description.ShouldBe("~~Updated by unit test~~")
            );

        var resp = await HttpClient!.DeleteAsync(new Uri($"{UriPrefix}/api/v1/{ScrantonClientId}/box/{id}"));
        resp.ShouldNotBeNull();
    }
}

