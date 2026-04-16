var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .WithHostPort(5432); // ← porta fixa sempre

var db = postgres.AddDatabase("ProductsDb");

var api = builder.AddProject<Projects.ProductsAPI_API>("api")
    .WithReference(db)
    .WaitFor(db);

builder.AddNpmApp("frontend", "../products-app")
    .WithReference(api)
    .WithHttpEndpoint(port: 4200, env: "PORT") // porta fixa
    .WaitFor(api);

builder.Build().Run();
