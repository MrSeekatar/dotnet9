var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                    .WithPersistence();

var api = builder.AddProject<Projects.BoxServerApi>("api")
    .WaitFor(cache)
    .WithReference(cache);

builder.AddProject<Projects.BoxUI>("ui")
    .WithExternalHttpEndpoints()
    .WaitFor(api)
    .WithReference(api);

builder.Build().Run();
