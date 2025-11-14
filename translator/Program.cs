
using translator.src.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Register gRPC channel to backend microservice
builder.Services.AddGrpcClients();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();  // exposes HTTP endpoints for nginx
app.Run();
