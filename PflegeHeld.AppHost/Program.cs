var builder = DistributedApplication.CreateBuilder(args);

var sqlServerPassword = builder.AddParameter("sqlPassword", secret: true, value: "P@ssword1");

var sql = builder.AddSqlServer("gamedevsconnect-backend-sql", port: 1400, password: sqlServerPassword)
                 .WithVolume("pflegeheld-sqlserver-data", "/var/opt/mssql");

//builder.AddProject<Projects.PflegeHeld_Backend_API_Personal>("pflegeheld-backend-api-personal");

var build = builder.Build();

await build.RunAsync();