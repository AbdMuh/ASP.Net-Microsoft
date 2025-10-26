using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory list to simulate a database
var users = new List<User>();

// CREATE - Add a new user
app.MapPost("/users", (User user) =>
{
    user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1; // Auto-increment Id using Max
    users.Add(user);
    return Results.Created($"/users/{user.Id}", user);
});

// READ - Get all users
app.MapGet("/users", () =>
{
    if(!users.Any())
        return Results.NotFound("No users exist in List.");
    return Results.Ok(users);
});

// READ - Get user by Id
app.MapGet("/users/{id:int}", (int id) =>
{
    var user = users.Find(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound("No User with this id found.");
});

// UPDATE - Update user by Id
app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    var user = users.Find(u => u.Id == id);
    if (user is null)
        return Results.NotFound("No User with this id found.");

    user.Username = updatedUser.Username;
    user.Age = updatedUser.Age;

    return Results.Ok(user);
});

// DELETE - Delete user by Id
app.MapDelete("/users/{id:int}", (int id) =>
{
    var user = users.Find(u => u.Id == id);
    if (user == null)
        return Results.NotFound("No User with this id found.");

    users.Remove(user);
    return Results.NoContent();
});

app.Run();

// User class
public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public int Age { get; set; }
}