var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:17")
    .WithDataVolume()
    .WithPgAdmin()
    .WithHostPort(5432);

var db = postgres.AddDatabase("ProductsDb");

var api = builder.AddProject<Projects.ProductsAPI_API>("api")
    .WithReference(db)
    .WithEnvironment("Jwt__Key", builder.Configuration["Jwt:Key"]!)
    .WithEnvironment("Jwt__Issuer", builder.Configuration["Jwt:Issuer"]!)
    .WithEnvironment("Jwt__Audience", builder.Configuration["Jwt:Audience"]!)
    .WithEnvironment("Jwt__ExpiresInMinutes", builder.Configuration["Jwt:ExpiresInMinutes"]!)
    .WaitFor(db);

builder.AddNpmApp("frontend", "../products-app")
    .WithReference(api)
    .WithHttpEndpoint(port: 4200, env: "PORT", isProxied: false)
    .WaitFor(api);

builder.Build().Run();
