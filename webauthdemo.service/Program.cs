using Microsoft.AspNetCore.Mvc.ApplicationParts;
using webauthdemo.fscontrollers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mvcConfig = builder.Services.AddControllers();
mvcConfig.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(FunController).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use API controllers with routing attributes
app.UseRouting();
// Discover controllers in all parts added above and map all routes to endpoints
app.MapControllers();

await app.RunAsync();