using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
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
// app.UseHttpLogging();


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
    var _logger=context.RequestServices.GetRequiredService<ILogger<Program>>();
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    myService.LogCreation("middleware");
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while processing the request.");
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





app.MapGet("/minimal/{id:int}", (int id) => $"Hello Minimal API {id}");
// app.MapGet("/minimal/{id:int:min(2)}/{name}", (int id, string name) => $"Hello Minimal API {id} {name}");
// app.MapGet("/minimal/{year?}", (int? year = 2016) => $"Hello Minimal API {year}");
// app.MapGet("/minimal/file/{*filepath}", (string filepath) => $"Hello Minimal API, filepath: {filepath}");
// app.MapGet("/minimal/store/{category}/{itemName?}/{itemId:int?}", (string category, string? itemName, int? itemId, bool? inStock) =>
// {
//     return $"Category: {category}, Item Name: {itemName ?? "N/A"}, Item ID: {itemId?.ToString() ?? "N/A"}, In Stock: {inStock?.ToString() ?? "N/A"}";
// });

var samplePerson = new Person { Id = 1, Name = "John Doe" };
app.MapGet("/manual-json", () =>
{
    var json = JsonSerializer.Serialize(samplePerson);
    return TypedResults.Text(json, "application/json");
});

app.MapGet("/custom-json", () =>
{
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper,
        WriteIndented = true
    };
    var json = JsonSerializer.Serialize(samplePerson, options);
    return TypedResults.Text(json, "application/json");
});

app.MapGet("/json", () => TypedResults.Json(samplePerson));
app.MapGet("/auto", () => samplePerson);

app.MapGet("/xml", () =>
{
    var xmlSerializer = new XmlSerializer(typeof(Person));
    var stringWriter = new StringWriter();
    xmlSerializer.Serialize(stringWriter, samplePerson);
    var xml = stringWriter.ToString();
    return TypedResults.Text(xml, "application/xml");
});

app.MapPost("/auto", (Person person) =>
{
    return TypedResults.Ok(person);
});

app.MapPost("/json", async (HttpContext context) =>
{
    var person = await context.Request.ReadFromJsonAsync<Person>();
    return TypedResults.Json(person);
});

app.MapPost("/custom-json", async (HttpContext context) =>
{
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };
    var person = await context.Request.ReadFromJsonAsync<Person>(options);
    return TypedResults.Json(person);
});

app.MapPost("/xml", async (HttpContext context) =>
{
    var xmlSerializer = new XmlSerializer(typeof(Person));
    var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync(); //body contains the xml string
    var stringReader = new StringReader(body); //xml string passed to StringReader
    var person = xmlSerializer.Deserialize(stringReader);
    return TypedResults.Ok(person);
});



app.Run();

public class Person
{
    public int Id { get; set; }
    public required string Name { get; set; }
}




