var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Memofee>("memofee");

builder.Build().Run();
