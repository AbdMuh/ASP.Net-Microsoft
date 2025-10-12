using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Remove builtin logging providers
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
// builder.Services.AddHttpLogging((options) => { });

builder.Services.AddScoped<IMyService, MyService>();
var app = builder.Build();

// app.UseHttpsRedirection();

app.UseAuthorization();



// app.Use(async (context, next) =>
// {
//     var myService = context.RequestServices.GetRequiredService<IMyService>();

//     Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
//     myService.LogCreation("middleware");
//     await next();
//     Console.WriteLine($"Response: {context.Response.StatusCode}");
// });
app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    myService.LogCreation("middleware");
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception caught in middleware: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
    }
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.MapControllers();

// app.Use(async (context, next) =>
// {
//     Console.WriteLine("Custom Middleware 1: Before next()");
//     await next();
//     Console.WriteLine("Custom Middleware 1: After next()");
// });

app.MapGet("/dependency", (IMyService myService) =>
{
    myService.LogCreation("Handling /middleware request");
    return Results.Ok("Dependency Injection worked!");
});



// app.UseHttpLogging();

app.MapGet("/minimal/{id:int}", (int id) => $"Hello Minimal API {id}");
// app.MapGet("/minimal/{id:int:min(2)}/{name}", (int id, string name) => $"Hello Minimal API {id} {name}");
// app.MapGet("/minimal/{year?}", (int? year = 2016) => $"Hello Minimal API {year}");
// app.MapGet("/minimal/file/{*filepath}", (string filepath) => $"Hello Minimal API, filepath: {filepath}");
// app.MapGet("/minimal/store/{category}/{itemName?}/{itemId:int?}", (string category, string? itemName, int? itemId, bool? inStock) =>
// {
//     return $"Category: {category}, Item Name: {itemName ?? "N/A"}, Item ID: {itemId?.ToString() ?? "N/A"}, In Stock: {inStock?.ToString() ?? "N/A"}";
// });
app.Run();




