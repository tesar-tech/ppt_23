using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ppt23.Api.Data;
using Ppt23.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsAllowedOrigin = builder.Configuration.GetSection("CorsAllowedOrigins").Get<string[]>();
ArgumentNullException.ThrowIfNull(corsAllowedOrigin);

builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(policy =>
    policy.WithOrigins(corsAllowedOrigin)//👈
    .WithMethods("GET", "DELETE", "POST", "PUT")//👈 (musí být UPPERCASE)
    .AllowAnyHeader()
));

string? sqliteDbPath = builder.Configuration[nameof(sqliteDbPath)];
ArgumentNullException.ThrowIfNull(sqliteDbPath);
builder.Services.AddDbContext<PptDbContext>(opt => opt.UseSqlite($"FileName={sqliteDbPath}"));
//někde za definicí proměnné app

var app = builder.Build();


app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<VybaveniVm> seznamVybaveni = VybaveniVm.VratRandSeznam(10);
List<RevizeVm> seznamRevizi = RevizeVm.VratRandSeznam(100);

app.MapGet("/vybaveni", (PptDbContext db) =>
{
    List<VybaveniVm> destinations = db.Vybavenis.ProjectToType<VybaveniVm>().ToList();
    return destinations;
});

app.MapGet("/vybaveni/{id}", (Guid id) =>
{
    VybaveniVm? en = seznamVybaveni.SingleOrDefault(x => x.Id == id);
    if (en is null)
        return Results.NotFound("Item Not Found!");
    return Results.Ok(en);
});

app.MapPost("/vybaveni", (VybaveniVm prichoziModel, PptDbContext db) =>
{
    prichoziModel.Id = Guid.Empty;
    var en = prichoziModel.Adapt<Vybaveni>();
    db.Vybavenis.Add(en);
    db.SaveChanges();
    return en.Id;
});


app.MapPut("/vybaveni", (VybaveniVm editedModel, PptDbContext db) =>
{
    var en = db.Vybavenis.SingleOrDefault(x => x.Id == editedModel.Id);

    if (en == null)
        return Results.NotFound("Item Not Found!");
    editedModel.Adapt(en);
    db.SaveChanges();
    return Results.Ok();
});


app.MapDelete("/vybaveni/{id}", (Guid id, PptDbContext db) =>
{
    var en = db.Vybavenis.SingleOrDefault(x => x.Id == id);
    if (en == null)
        return Results.NotFound("Item Not Found!");
    db.Vybavenis.Remove(en);
    db.SaveChanges();
    return Results.Ok();

});

app.MapGet("/revize/{text}", (string text) =>
{
    var filtrovaneRevize = seznamRevizi.Where(x => x.Name.Contains(text)).ToList();
    return Results.Ok(filtrovaneRevize);
});

using var appContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<PptDbContext>();
try
{
    //appContext.Database.OpenConnection();
    //appContext.Database.ExecuteSqlRaw("PRAGMA journal_mode=wal;");
    appContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception during db migration {ex.Message}");
    //throw;
}
app.Run();