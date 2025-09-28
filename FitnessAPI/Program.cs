using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using FitnessAPI.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FitnessDevContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("/api/users", async (FitnessDevContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

app.MapGet("/api/weightlogs", async (FitnessDevContext db) =>
{
    var weightLogs = await db.WeightLogs.ToListAsync();
    return Results.Ok(weightLogs);
});

app.MapGet("/api/FilteredWeightLogs/{id}", async (FitnessDevContext db, int id) =>
{
    var query = db.WeightLogs.AsQueryable();
    query = query.Where(x => x.UserId == id);
    var filteredWeightLog = await query.ToListAsync();

    return filteredWeightLog.Any() ? Results.Ok(filteredWeightLog.OrderByDescending(x => x.LogId)) : Results.NotFound();

});

app.MapPost("/api/Login", async (FitnessDevContext db, [FromBody] LoginModel login) =>
{
    if(string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
    {
        return Results.BadRequest("Username and password must be provided.");
    }
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == login.Username && u.Password == login.Password);
    return user != null ? Results.Ok(user) : Results.Unauthorized();
});

app.MapPost("/api/users", async (User user, FitnessDevContext db) =>
{
    if (await db.Users.AnyAsync(u => u.Username == user.Username))
    {
        return Results.Conflict("Username already exists.");
    }

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.UserId}", user);
});

app.Run();
