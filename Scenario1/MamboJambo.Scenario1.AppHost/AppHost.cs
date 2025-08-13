var builder = DistributedApplication.CreateBuilder(args);

// Reference existing Azure SQL Database instead of creating a local SQL Server container
var database = builder.AddConnectionString("MamboBankDb");

// Add the MamboBank Gateway WebApi project with database reference
var gateway = builder.AddProject<Projects.MamboBank_Gateway_WebApi>("mambobank-gateway")
    .WithReference(database);

builder.Build().Run();
