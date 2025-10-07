using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using FitnessAPI.Models;
using FitnessAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FitnessDevContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter your JWT Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {jwtSecurityScheme, Array.Empty<string>()}
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

//app.MapGet("/api/users", async (FitnessDevContext db) =>
//{
//    var users = await db.Users.ToListAsync();
//    return Results.Ok(users);
//});

//app.MapGet("/api/weightlogs", async (FitnessDevContext db) =>
//{
//    var weightLogs = await db.WeightLogs.ToListAsync();
//    return Results.Ok(weightLogs);
//});

//app.MapGet("/api/FilteredWeightLogs/{id}", async (FitnessDevContext db, int id) =>
//{
//    var query = db.WeightLogs.AsQueryable();
//    query = query.Where(x => x.UserId == id);
//    var filteredWeightLog = await query.ToListAsync();

//    return filteredWeightLog.Any() ? Results.Ok(filteredWeightLog.OrderByDescending(x => x.LogId)) : Results.NotFound();

//});

//app.MapPost("/api/Login", async (FitnessDevContext db, [FromBody] LoginModel login) =>
//{
//    if(string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
//    {
//        return Results.BadRequest("Username and password must be provided.");
//    }
//    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == login.Username && u.Password == login.Password);
//    return user != null ? Results.Ok(user) : Results.Unauthorized();
//});

//app.MapPost("/api/AuthorizedLogin", async (FitnessDevContext db, LoginModel loginRequest) =>
//{
//    JwtService _jwtService = new JwtService(db, new ConfigurationManager());
//    var result = await _jwtService.Authenticate(loginRequest);
//    return result != null ? Results.Ok(result) : Results.Unauthorized();
//});

//app.MapPost("/api/users", async (User user, FitnessDevContext db) =>
//{
//    if (await db.Users.AnyAsync(u => u.Username == user.Username))
//    {
//        return Results.Conflict("Username already exists.");
//    }

//    db.Users.Add(user);
//    await db.SaveChangesAsync();
//    return Results.Created($"/api/users/{user.UserId}", user);
//});

app.Run();
