using Microsoft.Extensions.Configuration;
using Ppt23.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsAllowedOrigin = builder.Configuration.GetSection("CorsAllowedOrigins").Get<string[]>();
ArgumentNullException.ThrowIfNull(corsAllowedOrigin);

Console.WriteLine($" CORS ALLOWED: {corsAllowedOrigin}");

builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(policy =>
    policy.WithOrigins(corsAllowedOrigin)//👈
    .WithMethods("GET","DELETE","POST","PUT")//👈 (musí být UPPERCASE)
    .AllowAnyHeader()
));
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

app.MapGet("/vybaveni", () => seznamVybaveni);

app.MapGet("/vybaveni/{id}", (Guid id) =>
{
    VybaveniVm? en = seznamVybaveni.SingleOrDefault(x => x.Id == id);
    if (en is null)
        return Results.NotFound("Item Not Found!");
    return Results.Ok(en);
});


app.MapPost("/vybaveni", (VybaveniVm prichoziModel) =>
{
    prichoziModel.Id = Guid.NewGuid();
    seznamVybaveni.Insert(0, prichoziModel);
    return prichoziModel.Id;
});

app.MapPut("/vybaveni", (VybaveniVm editedModel) =>
{

    var vybaveniVm_Entity = seznamVybaveni.SingleOrDefault(x => x.Id == editedModel.Id);
    if (vybaveniVm_Entity == null)
        return Results.NotFound("Item Not Found!");
    else
    {
        vybaveniVm_Entity.BoughtDateTime = editedModel.BoughtDateTime;
        vybaveniVm_Entity.LastRevisionDateTime = editedModel.LastRevisionDateTime;
        vybaveniVm_Entity.Name = editedModel.Name;

        return Results.Ok();
    }

});


app.MapDelete("/vybaveni/{id}", (Guid id) =>
{
    var item = seznamVybaveni.SingleOrDefault(x => x.Id == id);
    if (item == null)
        return Results.NotFound("Item Not Found!");
    seznamVybaveni.Remove(item);

    return Results.Ok();

});


app.MapGet("/revize/{text}", (string text) =>
{
    var filtrovaneRevize = seznamRevizi.Where(x => x.Name.Contains(text)).ToList();
    return Results.Ok(filtrovaneRevize);
});




app.Run();


