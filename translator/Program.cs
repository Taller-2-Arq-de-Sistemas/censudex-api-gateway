
using Microsoft.AspNetCore.Server.Kestrel.Core;
using translator.src.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(lo =>
    {
        lo.Protocols = HttpProtocols.Http1AndHttp2;
    });
});
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});
// Register gRPC channel to backend microservice
builder.Services.AddGrpcClients();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();  // exposes HTTP endpoints for nginx
app.Run();
