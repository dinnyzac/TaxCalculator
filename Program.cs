var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Add this line to register controller services

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers(); // Map controllers to endpoints
});

app.Run();
