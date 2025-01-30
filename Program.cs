using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.MapGet("/", async ([FromServices] TodoContext context) => 
{
    var list = await context.Items.ToListAsync();
    return Results.Ok(list);
});
app.MapPost("/", async ([FromServices] TodoContext context, [FromBody] Item item) =>
{
    context.Items.AddAsync(item);
    await context.SaveChangesAsync();
    return Results.Created($"/{item.Id}", item);
});
app.MapPut("/{id}", async ([FromServices] TodoContext context, [FromRoute] string id) => 
{
    Item item = await context.Items.FindAsync(int.Parse(id));
    item.IsComplete = !item.IsComplete;
    // context.Items.Update(item);
    await context.SaveChangesAsync();
    return Results.Ok();
});
app.MapDelete("/{id}", async ([FromServices] TodoContext context, [FromRoute] string id) =>
{
    Item item = await context.Items.FindAsync(int.Parse(id));
    context.Items.Remove(item);
    context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
