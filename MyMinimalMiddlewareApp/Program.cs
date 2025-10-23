var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestBody |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseBody |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();
    
var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // detailed errors
}
else
{
    app.UseExceptionHandler("/Home/Error"); // generic error page
}


// app.UseAuthentication();
// app.UseAuthorization();
app.UseHttpLogging();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    await next();
    Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});

app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    Console.WriteLine($"Start Time: {startTime}");
    await next();
    var duration = DateTime.UtcNow - startTime;
    Console.WriteLine($"Request processed in {duration.TotalMilliseconds} ms");
});
app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => { throw new Exception("Test exception"); });
app.MapGet("/Home/Error", () => "Oops! Something went wrong. Please try again later.");


app.Run();
