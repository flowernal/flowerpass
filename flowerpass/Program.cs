using flowerpass;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddDbContext<LoginContext>(options => options
    .UseNpgsql(connectionString)
    .UseSnakeCaseNamingConvention());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<LoginContext>();

    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

// GET /logins
app.MapGet("/logins", async (LoginContext db) =>
    await db.Logins.ToListAsync());

// GET /logins/{id}
app.MapGet("/logins/{id:int}", async (int id, LoginContext db) => 
    await db.Logins.FindAsync(id)
        is { } l
        ? Results.Ok(l)
        : Results.NotFound());

// POST /logins
app.MapPost("/logins", async (Login l, LoginContext db) =>
{
    db.Logins.Add(l);
    await db.SaveChangesAsync();

    return Results.Created($"/logins/{l.Id}", l);
});

// PUT /logins/{id}
app.MapPut("/logins/{id:int}", async (int id, Login l, LoginContext db) =>
{
    if (l.Id != id) return Results.BadRequest();

    var login = await db.Logins.FindAsync(id);

    if (login is null) return Results.NotFound();

    login.Website = l.Website;
    login.Username = l.Username;
    login.Password = l.Password;

    await db.SaveChangesAsync();
    
    return Results.Ok(login);
});

// DELETE /logins/{id}
app.MapDelete("/logins/{id:int}", async (int id, LoginContext db) =>
{
    var login = await db.Logins.FindAsync(id);

    if (login is null) return Results.NoContent();
        
    db.Logins.Remove(login);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();