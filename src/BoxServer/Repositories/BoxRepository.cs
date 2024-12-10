using CommunityToolkit.Diagnostics;
using BoxServer.Interfaces;
using BoxServer.Models;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Loyal.Core.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dommel;
using Dapper.FluentMap.Dommel.Mapping;
using MassTransit;

namespace BoxServer.Repositories;

public class BoxRepository : IBoxRepository
{
    private readonly ILogger<BoxRepository> _logger;
    private readonly IOptions<DbOptions> _options;

    private class BoxMap : DommelEntityMap<Box>
    {
        public BoxMap()
        {
            ToTable("BoxServer.Boxs");
        }
    }

    public BoxRepository(ILogger<BoxRepository> logger, IOptions<DbOptions> options
        )
    {
        _logger = logger;
        _options = options;

        FluentMapper.Initialize(c =>
        {
            c.AddMap(new BoxMap());
            c.ForDommel();
        });
    }

    public async Task<bool> DeleteBox(string clientId, Guid id)
    {
        using var conn = new DbConnectionEx(_options, clientId);

        return await conn.Connection.ExecuteAsync(@"DELETE FROM BoxServer.Boxs
                                                          WHERE BoxId = @id",
                                        new {id}).ConfigureAwait(false) == 1;
    }

    public async Task<Box?> GetBox(string clientId, Guid id)
    {
        using var conn = new DbConnectionEx(_options, clientId);
        var ret = await conn.Connection.GetAsync<Box>(id).ConfigureAwait(false);
        if ( ret is null)
            _logger.LogWarning("Box with id {id} for client {clientId} not found", id, clientId); // test for logging
        return ret;
    }

    public async Task<IEnumerable<Box>?> GetBoxs(string clientId)
    {
        using var conn = new DbConnectionEx(_options, clientId);
        return await conn.Connection.QueryAsync<Box>("SELECT * FROM BoxServer.Boxs").ConfigureAwait(false);
    }

    public async Task<Box> AddBox(string clientId, Box box)
    {
        Guard.IsNotNull(box);
        using var conn = new DbConnectionEx(_options, clientId);

        box.BoxId = NewId.NextGuid();
        box.CreatedOn = DateTime.UtcNow;

        const string sql = @"INSERT INTO [BoxServer].[Boxs] ([BoxId], [Name], [Description], [CreatedOn], [Active])
                                                             VALUES  (@Id,  @Name,  @Description,  @CreatedOn,  @Active);";
        await conn.Connection.ExecuteAsync(sql, new {Id = box.BoxId, box.Name, box.Description, box.CreatedOn, box.Active}).ConfigureAwait(false);
        return box;
    }

    public async Task<Box?> UpdateBox(string clientId, Box box)
    {
        using var conn = new DbConnectionEx(_options, clientId);

        return await conn.Connection.UpdateAsync(box).ConfigureAwait(false) ? box : null;
    }
}