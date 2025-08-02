var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PflegeHeld_Backend_API_Personal>("pflegeheld-backend-api-personal");

var build = builder.Build();

await build.RunAsync();