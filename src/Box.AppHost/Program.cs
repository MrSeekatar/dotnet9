var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.BoxServerApi>("api");

builder.AddProject<Projects.BoxUI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(api);


builder.Build().Run();
