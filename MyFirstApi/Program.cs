var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpLogging((options) => { });
var app = builder.Build();

// Configure the HTTP request pipeline.

// Remove HTTPS redirection so you can test with http

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Custom Middleware 1: Before next()");
    await next();
    Console.WriteLine("Custom Middleware 1: After next()");
});

app.UseHttpLogging();

app.MapGet("/minimal/{id:int}", (int id) => $"Hello Minimal API {id}");
app.MapGet("/minimal/{id:int:min(2)}/{name}", (int id, string name) => $"Hello Minimal API {id} {name}");
app.MapGet("/minimal/{year?}", (int? year = 2016) => $"Hello Minimal API {year}");
app.MapGet("/minimal/file/{*filepath}", (string filepath) => $"Hello Minimal API, filepath: {filepath}");
app.MapGet("/minimal/store/{category}/{itemName?}/{itemId:int?}", (string category, string? itemName, int? itemId, bool? inStock) =>
{
    return $"Category: {category}, Item Name: {itemName ?? "N/A"}, Item ID: {itemId?.ToString() ?? "N/A"}, In Stock: {inStock?.ToString() ?? "N/A"}";
});
app.Run();