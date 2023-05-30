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
builder.Services.AddScoped<SeedingData>();//for seeding data


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
    var allVybavenis = db.Vybavenis.Include(x => x.Revizes).ToList();
    List<VybaveniVm> vms = new();
    foreach (var vybaveni in allVybavenis)
    {
        var vm = vybaveni.Adapt<VybaveniVm>();
        vm.LastRevisionDateTime = vybaveni.Revizes.MaxBy(x => x.DateTime)?.DateTime;
        vms.Add(vm);
    }
    return vms;
});

app.MapGet("/vybaveni/{id}", (Guid id, PptDbContext db) =>
{
    Vybaveni? en = db.Vybavenis
    .Include(x=>x.Ukons).ThenInclude(x=>x.Pracovnik)
    .Include(x => x.Revizes).SingleOrDefault(x => x.Id == id);
    if (en is null)
        return Results.NotFound("Item Not Found!");
    return Results.Ok(en.Adapt<VybaveniSrevizemaVm>());
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

app.MapPost("revize", (RevizeVm vm, PptDbContext db) =>
{
    if (vm.VybaveniId == Guid.Empty) return Results.BadRequest();
    var en = vm.Adapt<Revize>();
    db.Revizes.Add(en);
    db.SaveChanges();
    return Results.Ok(en.Id);
});



app.MapPost("ukon", (UkonVm vm, PptDbContext db) =>
{
    if (vm.VybaveniId == Guid.Empty) return Results.BadRequest();
    var en = vm.Adapt<Ukon>();
    db.Ukons.Add(en);
    db.SaveChanges();
    return Results.Ok(en.Id);
});

app.MapDelete("/ukon/{id}", (Guid id, PptDbContext db) =>
{
    var en = db.Ukons.SingleOrDefault(x => x.Id == id);
    if (en == null)
        return Results.NotFound("Item Not Found!");
    db.Ukons.Remove(en);
    db.SaveChanges();
    return Results.Ok();

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

await app.Services.CreateScope().ServiceProvider.GetRequiredService<SeedingData>().SeedData();

app.Run();