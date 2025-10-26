using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Error handling middleware
app.UseExceptionHandler("/error");

//Authentication middleware
app.Use(async (context, next) =>
{
    // Extract token from request headers
    var token = context.Request.Headers["Authorization"].ToString();

    // Simple validation check (replace with real validation logic)
    if (string.IsNullOrWhiteSpace(token) || !IsValidToken(token))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized: Invalid or missing token." });
        return; 
    }
    await next();
});

// Helper method to validate tokens
static bool IsValidToken(string token)
{
    // Example: simple check
    return token == "my-secret-token"; 
}

//Logging middleware
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var path = context.Request.Path;

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception occurred: {ex.Message}");
        throw; // Re-throw so UseExceptionHandler handles it
    }

    var statusCode = context.Response.StatusCode;
    Console.WriteLine($"HTTP {method} {path} responded {statusCode}");
});




app.Map("/error", async (HttpContext context) =>
{
    context.Response.StatusCode = 500;

    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred. Please try again later." });
});

// In-memory list to simulate a database
var users = new List<User>();

// CREATE - Add a new user
app.MapPost("/users", (User user) =>
{           
    try
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            return Results.BadRequest("Username is required.");
        if (user.Age <= 0)
            return Results.BadRequest("Age must be a positive integer.");

        user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        return Results.Created($"/users/{user.Id}", user);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding user: {ex.Message}");
        return Results.Problem("An unexpected error occurred while adding the user.");
    }
});


// READ - Get all users
app.MapGet("/users", () =>
{
    try
    {
        if (!users.Any())
            return Results.NotFound("No users exist in the list.");

        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching users: {ex.Message}");
        return Results.Problem("An unexpected error occurred while fetching users.");
    }
});


// READ - Get user by Id
app.MapGet("/users/{id:int}", (int id) =>
{
    try
    {
        var user = users.Find(u => u.Id == id);
        return user is not null ? Results.Ok(user) : Results.NotFound("No User with this id found.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving user: {ex.Message}");
        return Results.Problem("An unexpected error occurred while fetching user data.");
    }
});


// UPDATE - Update user by Id
app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    try
    {
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return Results.NotFound($"No user with id {id} found.");

        if (string.IsNullOrWhiteSpace(updatedUser.Username))
            return Results.BadRequest("Username cannot be empty.");

        user.Username = updatedUser.Username;
        user.Age = updatedUser.Age;
        return Results.Ok(user);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating user: {ex.Message}");
        return Results.Problem("An unexpected error occurred while updating user.");
    }
});

// DELETE - Delete user by Id
app.MapDelete("/users/{id:int}", (int id) =>
{
    try
    {
        var user = users.Find(u => u.Id == id);
        if (user == null)
            return Results.NotFound("No User with this id found.");

        users.Remove(user);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting user: {ex.Message}");
        return Results.Problem("An unexpected error occurred while deleting user.");
    }
});


app.MapGet("/test-error", () =>
{
    return Results.Problem("Something went wrong while processing your request.");
});

app.MapGet("/throw-error", () =>
{
    throw new Exception("This is a test exception.");
});


app.Run();

// User class
public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public int Age { get; set; }
}