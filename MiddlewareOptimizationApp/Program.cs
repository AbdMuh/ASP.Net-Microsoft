var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();
    if(context.Response.StatusCode >= 404)
    {
        Console.WriteLine($"Security Event |[{DateTime.Now}] {context.Request.Method} {context.Request.Path} responded {context.Response.StatusCode}");
    }
});

app.Use(async (context, next) =>
{
    if (context.Request.Query["secure"] != "true")
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("HTTPS Request Required");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    // Check if the request path contains "/authorized"
    if (context.Request.Path.StartsWithSegments("/authorized"))
    {
        Console.WriteLine("Authorized route accessed!");
        await next();
    }
    else
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Forbidden: Path not allowed");
        return;
    }
});


app.Use(async (context, next) =>
{
    var input = context.Request.Query["input"];
    if (!IsValidInput(input))
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Input");
        }
        return;
    }

    await next();
});

// Helper method for input validation
static bool IsValidInput(string input)
{
    // Checks for any unsafe characters or patterns, including "<script>"
    return !string.IsNullOrEmpty(input) && (input.All(char.IsLetterOrDigit) && !input.Contains("<script>"));
}

app.Use(async (context, next) =>
{
    if (context.Request.Query["authorized"] == "true")
    {
        context.Response.Cookies.Append("AuthCookie", "AuthorizedUser", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
        });
        await next();
    }
    else
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden, since authorized=true not set");
        }
        return;
    }
});

app.Use(async (context, next) =>
{
    await Task.Delay(100); // Simulate async operation
    if (!context.Response.HasStarted)
    {
        await context.Response.WriteAsync("Processed Asynchronously\n");
    }
    await next();
});

app.Run();


// correct URL: http://localhost:5294/authorized/?secure=true&authorized=true&input=hello