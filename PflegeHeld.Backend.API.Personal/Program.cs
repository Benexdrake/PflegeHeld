using PflegeHeld.Backend.API.Configuration;

var start = new Startup("File");
var builder = start.Build(args);

//builder.Services.AddScoped<IFileRepository, FileRepository>();

//builder.Configuration["AZURE_STORAGE_BASE_URL"] = Environment.GetEnvironmentVariable("AZURE_STORAGE_BASE_URL");

var app = start.Create(builder);

//app.MapEndpointsV1();
app.Run();